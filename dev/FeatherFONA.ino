#include "Adafruit_FONA.h"
#include <SoftwareSerial.h>

#define FONA_RX  9
#define FONA_TX  8
#define FONA_RST 4
#define FONA_RI  7

#define LED 13

#define BUSYWAIT 5000  // milliseconds

// this is a large buffer for replies
char replybuffer[255];

SoftwareSerial fonaSS = SoftwareSerial(FONA_TX, FONA_RX);
SoftwareSerial *fonaSerial = &fonaSS;

Adafruit_FONA fona = Adafruit_FONA(FONA_RST);

boolean fonaInit(void) {
  Serial.begin(115200);
  Serial.println(F("FONA basic test"));
  Serial.println(F("Initializing....(May take 3 seconds)"));
 
  digitalWrite(LED, HIGH);
  delay(100);
  digitalWrite(LED, LOW);
  delay(100);
  digitalWrite(LED, HIGH);
  delay(100);
  digitalWrite(LED, LOW);
  delay(100);
     
  // make it slow so its easy to read
  fonaSerial->begin(4800);
  if (! fona.begin(*fonaSerial)) {
    Serial.println(F("Couldn't find FONA"));
    return false;
  }
  Serial.println(F("FONA is OK"));

  return true;
}

void setup() {
  // while (!Serial); // remove in production

  // set LED output for debugging
  pinMode(LED, OUTPUT);
 
  while (!fonaInit()) {
    delay(5000);
  }

  iniSMSInterrupt();

  printModuleIMEI();
  printMenu();
}

void iniSMSInterrupt() {
  pinMode(FONA_RI, INPUT);
  digitalWrite(FONA_RI, HIGH); // turn on pullup on RI
  // turn on RI pin change on incoming SMS
  fona.sendCheckReply(F("AT+CFGRI=1"), F("OK"));  
}

void printModuleIMEI() {
  char imei[16] = {0}; // MUST use a 16 character buffer for IMEI!
  uint8_t imeiLen = fona.getIMEI(imei);
  if (imeiLen > 0) {
    Serial.print("Module IMEI: "); Serial.println(imei);
  }
}

void printMenu(void) {
  Serial.println(F("-------------------------------------"));
  Serial.println(F("[b] read the Battery % and mV charged"));
  Serial.println(F("[i] read RSSI"));
  Serial.println(F("[l] query GSMLOC (GPRS)"));
  Serial.println(F("-------------------------------------"));
  Serial.println(F(""));
}

void loop() {
  digitalWrite(LED, HIGH);
  delay(100);
  digitalWrite(LED, LOW);
  delay(100);
  
  while (fona.getNetworkStatus() != 1) {
    Serial.println("Waiting for cell connection ...");
    delay(2000);
  }

  for (uint16_t i=0; i < BUSYWAIT; i++) {
    if (!digitalRead(FONA_RI)) {
      // RI pin went low, SMS received?
      Serial.println(F("RI went low"));
      break;
    }
    delay(1);
  }

  int8_t smsnum = fona.getNumSMS();
  if (smsnum < 0) {
    Serial.println(F("Failed to read # SMS!"));
    return;
  } else {
    Serial.print(smsnum); 
    Serial.println(F(" SMS's on SIM card"));
  }
   
  int8_t smsn = 1; // 1 indexed
  for ( ; smsn <= smsnum; smsn++) {
    digitalWrite(LED, HIGH);
    delay(100);
    digitalWrite(LED, LOW);
    delay(100);
    digitalWrite(LED, HIGH);
    delay(100);
    digitalWrite(LED, LOW);
    delay(100);
        
    // read request SMS
    uint16_t smslen;
    if (!fona.readSMS(smsn, replybuffer, 250, &smslen)) {  // pass in buffer and max len
      Serial.println(F("Failed to read SMS #!"));
      break;
    }
    // if the length is zero, its a special case where the index number is higher
    // so go to the next SMS slot
    if (smslen == 0) {
      Serial.println(F("[empty slot]"));
      continue;
    }

    char message[141];
    char command = replybuffer[0];
    switch (command) {
      case '\0':
          break;
      case 'b': {
          // read the battery voltage and percentage
          uint16_t vbat;
          char v[5] = "n/a";
          char p[4] = "n/a";
          if (fona.getBattVoltage(&vbat)) {
            snprintf(v, 5, "%4d", vbat);
          }
          if (fona.getBattPercent(&vbat)) {
            snprintf(p, 4, "%3d", vbat);
          }
          snprintf(message, 141, "VPct = %s %%, VBat = %s mV", p, v);
          break;
      }
      case 'i': {
          // read the RSSI
          uint8_t n = fona.getRSSI();
          int8_t r = 0;
          if (n == 0) r = -115;
          if (n == 1) r = -111;
          if (n == 31) r = -52;
          if ((n >= 2) && (n <= 30)) {
            r = map(n, 2, 30, -110, -54);
          }
          snprintf(message, 141, "RSSI = %4d: %4d dBm", n, r);
          break;
      }
      case 'l': {
          Serial.println(F("turn on GPRS!"));
          // turn GPRS on
          if (!fona.enableGPRS(true)) {
            Serial.println(F("Failed to turn on GPRS!"));
            break;
          }

          Serial.println(F("read GPS location"));
          // read GPS location
          uint16_t returncode;
          if (!fona.getGSMLoc(&returncode, message, 140)) {
            Serial.println(F("Failedto get GSM location!"));
          }
          else if (returncode != 0) {
            Serial.print(F("Fail code #")); Serial.println(returncode);
          }

          Serial.println(F("turn off GPRS!"));
          // turn GPRS off
          if (!fona.enableGPRS(false)) {
            Serial.println(F("Failed to turn off GPRS!"));
          }
          break;
      }
      default: {
          Serial.println(F("Unknown command"));
          break;
      }
    }

    // send SMS
    char sendTo[21];
    if (!fona.getSMSSender(smsn, sendTo, 20)) {
      Serial.println("Failed to retrieve SMS sender phone number!");
    }
    else if (!fona.sendSMS(sendTo, message)) {
      Serial.println(F("Failed to send SMS response!"));
    }

    // delete SMS
    if (!fona.deleteSMS(smsn)) {
      Serial.println(F("Failed to delete SMS!"));
    }
  }
}
