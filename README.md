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

