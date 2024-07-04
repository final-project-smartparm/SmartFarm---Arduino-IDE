# SmartFarm---Arduino-IDE
스마트팜 아두이노 개발 (하드웨어)

## day01
- ![ESP32 CAM 아두이노 웹캠]()
- 아두이노 IDE 설정   
    - 아두이노 IDE 설치  
    - 보드 매니저에서 ESP32 보드 추가 
    - 예제 코드 업로드 : 파일 > 예제 > ESP32 > Camera > CameraWebServer
    - 카메라 핀 설정 #define CAMERA_MODEL_AI_THINKER
    - Wi-Fi 설정 : 예제 코드에서 사용할 Wi-Fi SSID와 패스워드를 입력
        - ex : const char* ssid = "와이파이 이름";
        - ex : const chsar* password : "비밀번호";
    - 코드 업로드 
        - 도구 업로드 속도 1152000으로 설정 
        - 업로드 버튼을 누르고 코드 업로드 
    - 웹캠 스트리밍 확인
        - 시리얼 모니터 열기
            - 업로드가 완료된 후 , 도구 > 시리얼 모니터 , 115200 baud로 설정
            - 시리얼 모니터에서 ESP32-CAM의 IP주소 확인
    - 웹 브라우저 스트리밍 확인
            - 웹 브라우저에 시리얼 모니터에서 확인한 IP주소를 입력 
            - http://아이피:포트
            - 웹 페이지 스트리밍 확인

- ![공유기설정]()
- 공유기 기본 게이트위이 접속(http://192.168.5.1)으로 접속
    - 로그인 진행
        - 관리 로그인
    - 포트포워드 관리
        - 외부에서도 사용할 수 있게 외부포트 번호를 설정해줌 
    - LAN 연결 정보에서 범위를 변경
        - **공유기 상태 정보창에 나와있는 서브넷 마스크를 가져와서 사용**
        - IPAddress local_IP(172 , 30, 1, 5); //고정 아이피 
        - IPAddress gateway(172 , 30, 1, 254); //공유기 관리 주소
        - IPAddress subnet(255, 255, 255, 0); //서브넷 마크
        ```python 
        void setup() #부분에 추가
        if(!WiFi.config(local_IP, gateway, subnet)) {
            Serial.println("STA Failed to configure");
        }
        ```
        **추가해야 고정아이피 설정이 끝이 남**
- 공유기 상태 정보에서 인터넷 연결 정보의 ip 주소(외부 아이피)를 복사 후
    - 인터넷 창에 http://외부아이피:포트/mjpeg/1
    - 영상의 경우: http://외부아이피:포트/mjpeg/1
    - 사진의 경우: http://외부아이피:포트/jpg

## Arduino IDE 코드

1. 사용된 라이브러리
    1. Wire.h
        - 기능 : I2C 통신을 위한 라이브러리
        - 설명: 다른 I2C 장치와 Arduino 간의 데이터 통신을 담당
    ```c
    #include <Wire.h>
    ``` 
    2. LiquidCrystal_I2C.h
        - 기능: LCD 1602를 I2C 인터페이스로 제어하기 위한 라이브러리
        - 설명: 텍스트 및 기호를 LCD에 표시할 수 있음
    ```c
    #include <LiquidCrystal_I2C.h>
    ``` 
    3. Adafruit_NeoPixel.h
        - 기능: Adafruit사의 Neopixel (WS2812) LED를 제어하기 위한 라이브러리
        - 설명: RGB LED의 색상 및 밝기를 제어할 수 있음
     ```c
    #include <Adafruit_NeoPixel.h> 
    ```    
    4. DHT.h
        - 기능: DHT 시리즈(온습도 센서)를 제어하기 위한 라이브러리
        - 설명: 주변 환경의 온도와 습도를 측정할 수 있음
    ```c
    #include "DHT.h"
    ``` 

2. 정의된 상수 및 변수
    1. DHTPIN: DHT 센서의 데이터 핀을 A0로 정의
    ```c
    #define DHTPIN A0
    ``` 
    2. DHTTYPE: 사용할 DHT 센서 종류를 DHT11으로 정의
    ```c
    #define DHTTYPE DHT11
    ```     
    3. JODO_PIN: 주변 광 조도 센서가 연결된 핀을 A2로 정의
    ```c
    #define JODO_PIN A2
    ```     
    4. AA, AB, BA, BB: 모터 A와 B를 제어하기 위한 핀을 각각 5, 4, 6, 7로 설정
    ```c
    int AA = 5;  // 모터 A의 핀
    int AB = 4;  // 모터 A의 핀
    int BA = 6;  // 모터 B의 핀
    int BB = 7;  // 모터 B의 핀
    ``` 
    5. sensorPin: 다른 센서가 연결된 아날로그 핀 번호를 A2로 정의
    ```c
    const int sensorPin = A2;
    ``` 
    6. relayPin: 릴레이 제어 핀을 11로 설정
    ```c
    const int relayPin = 11;
    ```   
    7. 기타 변수
    ```c
    int sensorValue = 0;      // 센서 값 저장 변수
    int ON = 1;               // ON 상태를 나타내는 상수
    int OFF = 0;              // OFF 상태를 나타내는 상수
    const unsigned long motorOnTime = 5000;  // 모터가 켜져 있을 시간 (밀리초)
    const unsigned long motorOffTime = 5000; // 모터가 꺼져 있을 시간 (밀리초)
    unsigned long previousMillis = 0; // 마지막으로 모터 상태가 변경된 시간
    bool motorState = false; // 모터 상태 (켜짐: true, 꺼짐: false)
    int JODO;   // 조도 센서의 값을 저장할 변수
    float fMax_hum = 0.0;   // 습도 센서의 최대 기준값을 저장하는 변수(초기값 0.0)
    float fMax_temp = 0.0;  // 온도 센서의 최대 기준값을 저장하는 변수(초기값 0.0)
    float fMin_temp = 0.0;  // 온도 센서의 최소 기준값을 저장하는 변수(초기값 0.0)
    int Soil_moisture_reference = 0;    // 토양 수분 센서의 기준값을 저장하는 변수
    ```     

3. 객체 및 변수 초기화
    1. dht: DHT 센서 객체를 초기화하고 DHTPIN과 DHTTYPE을 사용하여 설정
    ```c
    DHT dht(DHTPIN, DHTTYPE);
    ```
    2. RGB_LED: Neopixel LED 객체를 초기화하고, 12개의 LED가 연결된 핀 번호 9를 NEO_GRB 방식으로 설정
    ```c
    Adafruit_NeoPixel RGB_LED = Adafruit_NeoPixel(12, 9, NEO_GRB);
    ```    
    3. lcd: LCD 객체를 초기화하고, I2C 주소 0x27을 사용하여 16x2 형식의 LCD를 설정
    ```c
    LiquidCrystal_I2C lcd(0x27, 16, 2);
    ``` 


4. setup() 함수
    1. 릴레이 핀 설정
    ```c
    pinMode(relayPin, OUTPUT); // 릴레이 핀을 출력으로 설정
    ```
    2. 릴레이 초기 상태 설정
    ```c
    digitalWrite(relayPin, LOW); // 릴레이 초기 상태를 꺼짐으로 설정
    ```
    3. 센서 기준값 설정
    ```c
    fMax_hum = 60.0;                            // 습도 센서 기준값 설정
    fMax_temp = 30.0;                           // 최대 온도 기준값 설정
    fMin_temp = 22.0;                           // 최소 온도 기준값 설정
    Soil_moisture_reference = 50;               // 토양 수분 기준값 설정 (0 ~ 100%)
    ```
    4. 주변 광 조도 센서 핀 설정
    ```c
    pinMode(JODO_PIN, INPUT);   // JODO_PIN으로 정의된 핀을 입력 모드로 설정
    ```
    5. 시리얼 통신 설정
    ```c
    Serial.begin(9600);     // 시리얼 통신을 초기화하고, 통신 속도를 9600 baud로 설정
    ```
    6. DHT 센서 초기화
    ```c
    dht.begin(); // DHT 초기화
    ```
    7. LCD 초기화
    ```c
    lcd.init();       // LCD 초기화
    lcd.backlight();  // LCD 백라이트 켜기
    ```
    8. Neopixel LED 초기화
    ```c
    RGB_LED.begin();            // Neopixel 초기화
    RGB_LED.setBrightness(200); // Neopixel 밝기 설정
    RGB_LED.show();             // Neopixel 표시
    ```
    9. 모터 핀 설정
    ```c
    pinMode(AA, OUTPUT);  // 모터 A 핀 설정
    pinMode(AB, OUTPUT);  // 모터 A 핀 설정
    pinMode(BA, OUTPUT);  // 모터 B 핀 설정
    pinMode(BB, OUTPUT);  // 모터 B 핀 설정
    ```

5. loop() 함수
    1. 주기적으로 실행되며, 주요 기능을 수행
    2. 릴레이를 제어하여 일정 시간 동안 모터를 켜고 끄는 작업을 수행
        - 모터가 켜져 있는 상태(motorState == true)
        ```c
        if (motorState) {
            if (currentMillis - previousMillis >= motorOnTime) {
                motorState = false; // 모터 상태를 꺼짐으로 설정
                previousMillis = currentMillis; // 현재 시간을 저장
                digitalWrite(relayPin, LOW); // 릴레이 OFF
            }
        }
        ```
        - 모터가 꺼져 있는 상태(motorState == false)
        ```c
        else {
            if (currentMillis - previousMillis >= motorOffTime) {
                motorState = true; // 모터 상태를 켜짐으로 설정
                previousMillis = currentMillis; // 현재 시간을 저장
                digitalWrite(relayPin, HIGH); // 릴레이 ON
            }
        }
        ```
    3. 다양한 센서 값(온습도, 토양 수분, 주변 광 조도)을 읽어들이고, 시리얼 모니터에 출력
        - 센서 값 읽기와 출력
        ```c
        JODO = analogRead(JODO_PIN);       // 주변 광 조도 센서 값 읽기
        float hum = dht.readHumidity();    // 습도값 읽기
        float temp = dht.readTemperature(); // 온도값 읽기
        int Soil_moisture = analogRead(A1); // 토양 수분값 읽기
        sensorValue = analogRead(sensorPin); // 센서 값 읽기

        int C_Soil_moisture = map(Soil_moisture, 1023, 0, 0, 100);   // 토양 수분값을 0~100으로 변환
        ```
        - map() 함수
        ```c
        int C_Soil_moisture = map(Soil_moisture, 1023, 0, 0, 100);  // Soil_moisture 변수의 값(아날로그 값)을 0에서 100 사이의 값으(백분율)로 변환하여 C_Soil_moisture 변수에 저장
        ```
        - 시리얼 모니터 출력
        ```c
        Serial.print("Soil_moisture : ");
        Serial.println(C_Soil_moisture);
        Serial.print("temp:");
        Serial.print(dht.readTemperature());
        Serial.print(",");
        Serial.print("humid:");
        Serial.println(dht.readHumidity());
        Serial.print(" 주변 광 조도 센서 값 : ");
        Serial.println(JODO);
        Serial.print("Water Level: ");
        Serial.println(sensorValue);
        ```
    4. 토양 수분이 일정 기준보다 낮을 경우 LCD에 경고 메시지를 출력
        ```c
        if (C_Soil_moisture < Soil_moisture_reference)  // 수분 값이 기준보다 낮을 경우
        {
        WarrningLCD();  // LCD 경고 표시 함수 호출
        }
        else
        {
        lcd.setCursor(0, 0);
        lcd.print("T : "); lcd.print((int)temp); lcd.print("C ");
        lcd.setCursor(8, 0);
        lcd.print("H : "); lcd.print((int)hum); lcd.print("% ");
        lcd.setCursor(0, 1);
        lcd.print("Soil_M : "); lcd.print(strC_Soil); lcd.print("   ");
        }
        ```
    5. 온습도에 따라 팬 및 LED를 제어하여 환경 조건을 유지
        ```c
        if ((temp > fMax_temp && hum > fMax_hum) || temp > fMax_temp)  // 온습도 둘 다 높거나 온도만 높을 때
        {
        FanONOFF(ON);  // 팬 ON
        RGB_Color(RGB_LED.Color(0, 0, 0), 10);  // LED OFF (색상: 검정)
        }
        else if (hum > fMax_hum)  // 습도가 높을 때
        {
        FanONOFF(ON);  // 팬 ON
        RGB_Color(RGB_LED.Color(100, 100, 100), 10);  // LED OFF (색상: 회색)
        }
        else if (temp < fMin_temp)  // 온도가 낮을 때
        {
        FanONOFF(OFF);  // 팬 OFF
        RGB_Color(RGB_LED.Color(100, 100, 100), 10);  // LED ON (색상: 회색)
        }
        else
        {
        FanONOFF(OFF);  // 팬 OFF
        RGB_Color(RGB_LED.Color(0, 0, 0), 10);  // LED OFF (색상: 검정)
        }
        ```

6. 사용자 정의 함수
    1. RGB_Color: Neopixel LED의 색상을 설정
        ```c
        void RGB_Color(float c, int wait) {
        for (int i = 0; i < RGB_LED.numPixels(); i++) {  
            RGB_LED.setPixelColor(i, c);  // 네오픽셀 LED의 각 픽셀에 색상 `c`를 설정
            RGB_LED.show();  // 설정한 색상을 실제로 픽셀에 적용
            delay(wait);  // 지정된 시간(ms) 동안 대기
        }
        }
        ```
    2. int2String: 정수 값을 문자열로 변환하여 자릿수에 따라 표시
        ```c
        String int2String(int val) {
        String sval = "";

        int val2 = (val / 10) % 10;  // 10의 자리 숫자 추출
        int val3 = (val / 100) % 10; // 100의 자리 숫자 추출
        
        if (val3 >= 1) {
            sval = String(val);  // 세 자리 수 이상일 때, 그대로 문자열로 변환
        } else {
            if (val2 >= 1) {
            sval = " " + String(val);  // 두 자리 수일 때, 앞에 공백을 추가하여 문자열로 변환
            } else {
            sval = "  " + String(val);  // 한 자리 수일 때, 두 개의 공백을 추가하여 문자열로 변환
            }
        }

        return sval;    // 변환된 문자열을 반환
        }
        ```
    3. WarrningLCD: LCD에 물 부족 경고 메시지를 출력
        ```c
        void WarrningLCD() { 
            lcd.clear();  // LCD 화면 지우기
            delay(200);   // 200ms 대기
            lcd.backlight();  // LCD 백라이트 켜기
            lcd.setCursor(0, 0);  // 커서 위치 설정: 첫 번째 줄의 첫 번째 열
            lcd.print("Not enough water");  // 첫 번째 줄에 "Not enough water" 출력
            lcd.setCursor(0, 1);  // 커서 위치 설정: 두 번째 줄의 첫 번째 열
            lcd.print("supply water!!");  // 두 번째 줄에 "supply water!!" 출력
            delay(200);  // 200ms 대기
        }
        ```
    4. FanONOFF: 팬을 제어하여 온도에 따라 켜고 끔
        ```c
        void FanONOFF(int OnOff) {
            if (OnOff == 1) {
                digitalWrite(AA, HIGH);  // AA 핀을 HIGH로 설정하여 모터 A를 작동
                digitalWrite(AB, LOW);   // AB 핀을 LOW로 설정하여 모터 A의 방향을 설정
                digitalWrite(BA, HIGH);  // BA 핀을 HIGH로 설정하여 모터 B를 작동
                digitalWrite(BB, LOW);   // BB 핀을 LOW로 설정하여 모터 B의 방향을 설정
            } else {
                digitalWrite(AA, LOW);   // AA 핀을 LOW로 설정하여 모터 A를 정지시킴
                digitalWrite(AB, LOW);   // AB 핀을 LOW로 설정하여 모터 A의 방향을 정지시킴
                digitalWrite(BA, LOW);   // BA 핀을 LOW로 설정하여 모터 B를 정지시킴
                digitalWrite(BB, LOW);   // BB 핀을 LOW로 설정하여 모터 B의 방향을 정지시킴
            }
        }
        ```