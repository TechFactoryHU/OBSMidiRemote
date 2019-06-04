/*
 *  StreamerBox for OBS Studio
 *  HW needed: 
 *  - Arduino leonardo for MIDI communication 
 *  - Any other microcontroller for midi over SERIAL communication (arduino uno/esp8266/etc)
 *  
 *  Software needed:
 *  - OBS Studio
 *  - OBS Websocket plugin
 *  - OBSMidiRemote windows 
 *  Author: techfactory.hu, 2019
 *  3rd parties: Arduino, MidiUSB, Adafruit MCP23017, Adafruit Neopixel 
 */

//debug mode (Messages will send out over serial)
//#define TFDEBUG 1

//communication mode
//choose midi OR serial
//#define COM_MIDI 1
#define COM_SERIAL 1

//buttons (20 + 1 dummy for status led)
#define BUTTONS 21
//20 buttons + 1 status led (tflogo)
#define LEDS 21
//max colors count
#define COLORS 31

#include <Wire.h>
#include "Adafruit_MCP23017.h"
#include <Adafruit_NeoPixel.h>
#ifdef COM_MIDI 
  #include "MIDIUSB.h"
#endif

//using mcp for io extender
Adafruit_MCP23017 mcp;
Adafruit_NeoPixel pixels = Adafruit_NeoPixel(LEDS, 21, NEO_GRB + NEO_KHZ800);

struct MIDIBUTTON {
    byte type;
    byte btn_address;
    byte led_address;
    byte status;
    byte color;
    byte ledmode;
};

MIDIBUTTON MidiButtons[BUTTONS];

struct RGB {
  byte r;
  byte g;
  byte b;
};

//midi commands & types
enum MidiType {
    InvalidType           = 0x00,    ///< For notifying errors
    HandShake             = 0x10,    ///< Handshake (only for serial based controllers)
    NoteOff               = 0x80,    ///< Note Off
    NoteOn                = 0x90,    ///< Note On
    AfterTouchPoly        = 0xA0,    ///< Polyphonic AfterTouch
    ControlChange         = 0xB0,    ///< Control Change / Channel Mode
    ProgramChange         = 0xC0,    ///< Program Change
    AfterTouchChannel     = 0xD0,    ///< Channel (monophonic) AfterTouch
    PitchBend             = 0xE0,    ///< Pitch Bend
    SystemExclusive       = 0xF0,    ///< System Exclusive
    TimeCodeQuarterFrame  = 0xF1,    ///< System Common - MIDI Time Code Quarter Frame
    SongPosition          = 0xF2,    ///< System Common - Song Position Pointer
    SongSelect            = 0xF3,    ///< System Common - Song Select
    TuneRequest           = 0xF6,    ///< System Common - Tune Request
    Clock                 = 0xF8,    ///< System Real Time - Timing Clock
    Start                 = 0xFA,    ///< System Real Time - Start
    Continue              = 0xFB,    ///< System Real Time - Continue
    Stop                  = 0xFC,    ///< System Real Time - Stop
    ActiveSensing         = 0xFE,    ///< System Real Time - Active Sensing
    SystemReset           = 0xFF,    ///< System Real Time - System Reset
};

struct MidiPacket {
  byte cmd;
  byte channel;
  byte data1;
  byte data2;
};

RGB colors[COLORS];
bool ledchanged = false;
bool animactive = false;
int frame = 0;
const int frame_time = 30; //30ms
unsigned long start_time = 0; //millis
int anim_length = 100;  //in frame

void setup() {
  #if defined(TF_DEBUG) || defined(COM_SERIAL)
  Serial.begin(115200);
  Serial.setTimeout(50);
  #endif
  
  colors[0] = {0,0,0};
  colors[1] = {255,0,0};  //red
  colors[2] = {0,255,0};  //green
  colors[3] = {0,0,255};  //blue
  colors[4] = {255,255,0};//orange
  colors[5] = {255,0,255};//violet
  colors[6] = {0,255,255};//cyan
  colors[7] = {150,0,255};//purple
  colors[8] = {255,255,255};//white
  colors[9] = {255,150,0};//light-orange
  colors[10] = {0,150,255};//light-blue

  colors[11] = {127,0,0};
  colors[12] = {0,127,0};
  colors[13] = {0,0,127};
  colors[14] = {127,127,0};
  colors[15] = {127,0,127};
  colors[16] = {0,127,127};
  colors[17] = {75,0,127};
  colors[18] = {127,127,127};
  colors[19] = {127,75,0};
  colors[20] = {0,75,127};

  colors[21] = {63,0,0};
  colors[22] = {0,63,0};
  colors[23] = {0,0,63};
  colors[24] = {63,63,0};
  colors[25] = {63,0,63};
  colors[26] = {0,63,63};
  colors[27] = {37,0,63};
  colors[28] = {63,63,63};
  colors[29] = {63,37,0};
  colors[30] = {0,37,63};
  
  //MCP driven buttons
  mcp.begin();
  // use default address 0
  
  //type, btn_address, led_address, status
  MidiButtons[0] = {1,0,16,0};
  MidiButtons[1] = {1,1,15,0};
  MidiButtons[2] = {1,2,14,0};
  MidiButtons[3] = {1,3,13,0};

  MidiButtons[4] = {1,4,17,0};
  MidiButtons[5] = {1,5,18,0};
  MidiButtons[6] = {1,6,19,0};
  MidiButtons[7] = {1,7,20,0};

  /*
  MidiButtons[0] = {1,0,17,0};
  MidiButtons[1] = {1,1,18,0};
  MidiButtons[2] = {1,2,19,0};
  MidiButtons[3] = {1,3,20,0};

    MidiButtons[4] = {1,4,16,0};
  MidiButtons[5] = {1,5,15,0};
  MidiButtons[6] = {1,6,14,0};
  MidiButtons[7] = {1,7,13,0};
  */
  
  MidiButtons[8] = {1,8,9,0};
  MidiButtons[9] = {1,9,10,0};
  MidiButtons[10] = {1,10,11,0};
  MidiButtons[11] = {1,11,12,0};
  
  //ATmega/Leonardo driven buttons
  MidiButtons[12] = {0,20,8,0}; //11
  MidiButtons[13] = {0,19,7,0}; //10
  MidiButtons[14] = {0,18,6,0}; //9
  MidiButtons[15] = {0,9,5,0};  //8

  MidiButtons[16] = {0,8,1,0};  //6
  MidiButtons[17] = {0,7,2,0};  //5
  MidiButtons[18] = {0,6,3,0};  //4
  MidiButtons[19] = {0,5,4,0};  //3
  //its only for logo light
  MidiButtons[20] = {2,0,0,0};

  //setup buttons
  for (int i=0; i<BUTTONS; i++) {
    if (MidiButtons[i].type == 1) {
      mcp.pinMode(MidiButtons[i].btn_address, INPUT);
      mcp.pullUp(MidiButtons[i].btn_address, HIGH);
    }else {
      pinMode(MidiButtons[i].btn_address, INPUT_PULLUP);
    }
    
    //turn off = initial state
    MidiButtons[i].ledmode = 0;
    MidiButtons[i].color = 0;
    MidiButtons[i].status = 0;
  }
  
  //NeoPixels
  pixels.begin();
  //turn status led on
  pixels.setPixelColor(0, pixels.Color(255,0,0));
  ledchanged = true;
}

void parseMidiMessage(MidiPacket mididata) {
    //noteOn
    if (mididata.cmd == MidiType::NoteOn) {
        uint8_t btn_index = mididata.data1 - 10;
        uint8_t color_index = mididata.data2;
        if (color_index >= COLORS || color_index < 0) {
           color_index = 0;
        }
        uint8_t ledmode = mididata.channel;
        //button lights
        if (btn_index >= 0 && btn_index < BUTTONS) {
           if (ledmode == 1) {
               setButtonColor(btn_index,color_index);
           }else {
               MidiButtons[btn_index].color   = color_index; 
               MidiButtons[btn_index].ledmode = ledmode; 
           }
           ledchanged = true;
        }
    //noteoff
    }else if(mididata.cmd == MidiType::NoteOff) {
       int btn_index = mididata.data1 - 10;
       //button light
       if (btn_index >= 0 && btn_index <= BUTTONS) {
          setButtonColor(btn_index,0);
       }
    }
    #if defined(COM_SERIAL)
    else if(mididata.cmd == MidiType::HandShake) {
        sendMidiCmd(MidiType::HandShake, mididata.data2, mididata.data1, mididata.channel);
    }
    #endif
}

byte midi_in_buffer[4];
uint8_t midi_in_pointer = 0;

void ReceiveMessage(){
   MidiPacket mididata;
   #if defined(COM_MIDI)
      midiEventPacket_t rx;
      rx = MidiUSB.read();
      if (rx.header != 0) {
          mididata.cmd = rx.header << 4;
          mididata.channel = rx.byte1 & 0x0f;
          mididata.data1 = rx.byte2 & 0x7f;
          mididata.data2 = rx.byte3 & 0x7f;
          parseMidiMessage(mididata);
      }
   #elif defined(COM_SERIAL)
      if (Serial.available()>0) {
          byte c = Serial.read();
          if (c == 0x4) {
              mididata.cmd     = midi_in_buffer[0] << 4;
              mididata.channel = midi_in_buffer[1] & 0x0f;
              mididata.data1   = midi_in_buffer[2];
              mididata.data2   = midi_in_buffer[3];
              for (int i=0; i<4; i++) {
                  midi_in_buffer[i] = 0x00;
              }
             /* Serial.print(midi_in_pointer);
              Serial.print(":");
              Serial.print(mididata.cmd,HEX);
              Serial.print(" ");
              Serial.print(mididata.channel,HEX);
              Serial.print(" ");
              Serial.print(mididata.data1,HEX);
              Serial.print(" ");
              Serial.print(mididata.data2,HEX);
              Serial.println(" ");
              Serial.write(0x4);
             */
              if (midi_in_pointer == 4) {
                //Serial.println("Parsing...");
                parseMidiMessage(mididata);
              }
              midi_in_pointer = 0;
          }else {
              if (midi_in_pointer > 3) { midi_in_pointer = 0; }
              midi_in_buffer[midi_in_pointer] = c;
              midi_in_pointer++;
          }
      }
   #endif
}

void loop() {
   if (!start_time||millis()<start_time) { start_time = millis(); }
   animactive = false;
   
   uint8_t st = 0;
   for (int i=0; i<BUTTONS; i++) {
      //mcp buttons
      if (MidiButtons[i].type == 1) {
        st = mcp.digitalRead(MidiButtons[i].btn_address);
        checkButtonAction(i, st);
      //atmega buttons
      }else if(MidiButtons[i].type == 0) {
         st = digitalRead(MidiButtons[i].btn_address);
         checkButtonAction(i, st);
      }
      
      if (MidiButtons[i].ledmode > 1) {
           if (!animactive) {
              frame = floor((millis() - start_time)/frame_time); 
              animactive = true;
           }
           animButtonColor(i);
           //restart frame counter
           if (frame > anim_length) { start_time = millis(); }
      }else if(MidiButtons[i].ledmode == 1 && MidiButtons[i].color == 0) {
          setButtonColor(i, 0);
      }
   }
   
   if (!animactive) {
      start_time = 0; frame = 0;
   }
   
   if (ledchanged) {
      pixels.show();
      ledchanged = false;
   }
   
   ReceiveMessage();
}

RGB getColor(uint8_t index) {
    if (index >=0 && index < COLORS) {
        return colors[index];
    }
    return colors[0];
}

void sendMidiCmd(MidiType cmd, byte data1, byte data2, byte channel) {
    #if defined(COM_MIDI)
       midiEventPacket_t event = {cmd >> 4, cmd | channel, data1, data2};
       MidiUSB.sendMIDI(event);
       MidiUSB.flush();
    #elif defined(COM_SERIAL)
       Serial.write(cmd >> 4);
       Serial.write(cmd | channel);
       Serial.write(data1);
       Serial.write(data2);
       Serial.write(0x4); //EOT
       Serial.flush();
    #endif
}

uint8_t getButtonColor(uint8_t btn_index) {
    return MidiButtons[btn_index].color;
}

void setButtonColor(uint8_t btn_index, uint8_t color_index) {
    if (MidiButtons[btn_index].ledmode == 1 && MidiButtons[btn_index].color == color_index) {
        return;
    }
    MidiButtons[btn_index].ledmode = 1;
    MidiButtons[btn_index].color = color_index;
    RGB c = getColor(color_index);
    pixels.setPixelColor(MidiButtons[btn_index].led_address, pixels.Color(c.r,c.g,c.b));
    ledchanged = true;
}

void checkButtonAction(uint8_t btn_index, uint8_t st) {
  if (MidiButtons[btn_index].status != st) {
     if (st == 0) {
        setButtonColor(btn_index, 28); //visual feedback
        sendMidiCmd(MidiType::NoteOn, btn_index+10, 127, 1);
        //Serial.println(String(btn_index)+" ON");
     }else {
        if (getButtonColor(btn_index) == 28) { setButtonColor(btn_index, 0); } //visual feedback
        sendMidiCmd(MidiType::NoteOff, btn_index+10, 0, 1);
        //Serial.println(String(btn_index)+" OFF");
     }
     MidiButtons[btn_index].status = st;
  }
}

void animButtonColor(uint8_t btn_index) {
  RGB tmp;
  int a_frames = 1;
  int spd = floor(MidiButtons[btn_index].ledmode / 10)+1;
  int a_type = MidiButtons[btn_index].ledmode % 10;
  a_frames = floor(anim_length / spd);             
  int checkFrame = frame % a_frames;
  
    
  //blink
  if (a_type == 2) {
     if (checkFrame <= floor(a_frames/2)) {
       tmp = getColor(MidiButtons[btn_index].color);
     }else {
       tmp = RGB{0,0,0};
     }
  }
  //flash
  else if(a_type == 3) {
      float p = ((float)checkFrame/(float)a_frames);
      tmp = fadeColor(getColor(MidiButtons[btn_index].color), RGB{0,0,0}, p);
  }
  //flash-reverse
  else if(a_type == 4) {
      float p = ((float)checkFrame/(float)a_frames);
      tmp = fadeColor(getColor(MidiButtons[btn_index].color), RGB{0,0,0}, (1 - p));
  }
  //breathe
  else if(a_type == 5) {
      float p;
      p = (float)checkFrame/(float)(a_frames/2);
      if (p<=1) { 
          tmp = fadeColor(RGB{0,0,0}, getColor(MidiButtons[btn_index].color), p);
      }else {
          p = 1 - (p-1);
          tmp = fadeColor(RGB{0,0,0}, getColor(MidiButtons[btn_index].color), p);
      }
  }
  pixels.setPixelColor(MidiButtons[btn_index].led_address, pixels.Color(tmp.r,tmp.g,tmp.b));
  ledchanged = true;
}

RGB fadeColor(RGB a, RGB b, float p) {
  RGB c;
  c.r = _fadeColorHelper(a.r, b.r, p);
  c.g = _fadeColorHelper(a.g, b.g, p);
  c.b = _fadeColorHelper(a.b, b.b, p);
  return c;
}

uint8_t _fadeColorHelper(int a, int b, float p) {
  int diff = a - b;
  return diff != 0 ? constrain( ceil(a - (diff * p)), 0, 255) : a;
}
