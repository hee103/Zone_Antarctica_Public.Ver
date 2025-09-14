<img src="https://readme-decorate.vercel.app/api/get?type=star&text=Zone:Antarctica&width=1200&height=200&fontSize=80&fontWeight=800&useGradient=true&fontColor=%23FFFFFF&backgroundColor=%23c9c9c9&gradientColor1=%23FFFFFF&gradientColor2=%230000FF">


# 🛠️ Description
Zone_Antarctica 프로젝트의 유료 에셋 저작권 문제로 소스코드만 올리는 Repository입니다.
- **프로젝트 소개** <br>
  엉망이 된 기지 내부와 정체불명의 모습으로 변해버린 동료들 그리고 극한의 환경 <br>
  살아남으려면 자원을 수집하고 장비를 제작해야 합니다. <br>
  또한 배를 수리할 부품과 연료를 제작하여 이곳에서 탈출하는 것이 목표입니다.
<br>

- **개발 기간** : 2025.04.02 - 2025.06.02
- **개발 인원** : 5인 개발
<br><br>
<br><br>
---
# 🤝 팀원(역할 분담)

| 진희원 | 김진홍 | 원대건 | 박호준 | 천지훈 |
|----|----|----|----|----|
| 동물, 몬스터 AI | 인벤토리 | 회원가입, 로그인 | 건축 시스템 | 플레이어 |
| 스토리 씬 | 사운드 매니저 | 제작, 저장 | 지형 생성 | 날씨 |

<br><br>
<br><br>

---
# 🎮 프로젝트 소개 영상

[![프로젝트 영상](https://youtu.be/Lwh7YiFHHyY.jpg)](https://youtu.be/Lwh7YiFHHyY)

</a>


<br><br>
<br><br>

---
# 🎲 주요 기능
  
</details>
<details><summary>회원가입/ 로그인 시스템</summary>

### 회원가입/ 로그인 시스템 
<img src="[https://github.com/user-attachments/assets/884d293e-eaab-4657-aa64-f067b0f1cf66](https://teamsparta.notion.site/image/attachment%3A9a91930b-5e77-4ab8-828d-ef55eeb868dd%3Aimage.png?table=block&id=2012dc3e-f514-80b5-b2a6-d144b9ff5c77&spaceId=83c75a39-3aba-4ba4-a792-7aefe4b07895&width=1530&userId=&cache=v2)" width="400" height="300">

- Firebase Realtime Database를 활용한 사용자 정보 저장 및 조회
- 이메일 기반의 로그인 / 회원가입 시스템 구현
- UI 상에서 입력 검증, 중복 검사, 예외 처리 등 UX 개선 요소 반영
- 데이터 저장 시 Push 키 기반 고유 식별자 생성으로 사용자별 분리 관리

---
<br><br>
  
</details>
<details><summary>저장 시스템</summary>
  
<img src="[https://github.com/user-attachments/assets/51326850-9efd-45f8-a580-ce3391be34ac](https://teamsparta.notion.site/image/attachment%3A0c933cc0-ddce-4110-8cf1-47894516de3f%3Aimage.png?table=block&id=2012dc3e-f514-8069-86f5-d2716a135fa0&spaceId=83c75a39-3aba-4ba4-a792-7aefe4b07895&width=1530&userId=&cache=v2)" width="400" height="300">

### 저장 시스템
- 게임의 진행 상황을 슬롯 단위로 저장/불러오기 가능하도록 구성
- 저장 슬롯은 유저 키 + 슬롯 번호 조합으로 유일하게 식별
- 저장 후 게임 재시작 시 해당 슬롯 데이터로 즉시 적용
- Firebase Realtime Database를 기반으로 클라우드 저장 기능 구현
```
    - 퀵슬롯 구현 → 각 슬롯에 Item Data를 넣어놓는 방식
    - 1 ~ 9번 키를 입력 → 해당 index에 있는 item Data 찾아서 플레이어에 적용
    - 손에 쥘 수 있는 Prefab이 있다면 생성하여 사용
```

---
<br><br>
  
</details>
<details><summary>퀘스트 시스템</summary>
  
### 지형 시스템
<img src="https://github.com/user-attachments/assets/504e9da7-d243-408b-af3c-a25efbf96ea8" width="400" height="300">

대규모 월드의 동적 지형 생성

- 월드는 Voxel 기반으로 구성, 지형은 플레이어 주변만 동적으로 생성
- 성능을 위해 월드를 청크 단위로 분할하여 로드/언로드 -> 메쉬 데이터 생성시에도 이득

비동기 지형 처리 (멀티스레딩)

- 청크 생성과 로딩은 백그라운드 스레드에서 실행되어 메인스레드 성능을 보호
- 작업 완료 후 MainThreadDispatcher를 통해 Unity API 호출을 메인스레드에서 안전하게 진행

청크 -> 리전 단위 저장 구조

- 청크들을 리전 단위로 묶어 한 파일로 저장
- 디스크 IO 횟수 감소, 용량 절약

런타임 지형 편집

- 복셀 밀도 값을 동적으로 수정 가능
- 밀도 값이 바뀌면 메쉬를 재생성하여 렌더링

쉐이더 연동 텍스쳐 블렌딩

- 복셀의 밀도 정보를 Vertex Color로 전달하여 Shader Graph에서 눈/흙 텍스쳐를 혼합
- 지형 상태를 시각적으로 피드백

필터 기반 지형 생성 시스템

- ScriptableObject 기반의 필터 체인 시스템 구성
- 예: Perlin 노이즈 강도, 높이 제한, 경사도 제한 등의 필터를 조합해 다양한 지형 생성
```
  - 퀘스트 시스템 구현 → QuestData에 퀘스트의 Type과 Target을 넣었습니다.
  - ex) 나무 10개 습득 → Type : Pickup, Target : 나무, targetCnt : 10
  - 아이템 습득이 일어나는 곳에서 해당 Type과 Target을 QuestManager에 보고
  - QuestManger에서 퀘스트 진행도 체크
  - 완료 되었다면 보상 지급 및 다음 퀘스트 진행
```


---
<br><br>

</details>
<details><summary>플레이어 </summary>

### 플레이어 컨디션

<img src="[https://github.com/user-attachments/assets/bba0b64c-63ad-4952-a681-35e538a4462b](https://teamsparta.notion.site/image/attachment%3A2531cd2f-1ffa-47d3-b24d-d85b5ba7ba51%3Aimage.png?table=block&id=2022dc3e-f514-807b-8744-d58822340d3d&spaceId=83c75a39-3aba-4ba4-a792-7aefe4b07895&width=1530&userId=&cache=v2)" width="400" height="300">

- 배고픔, 갈증, 추위 등의 수치가 **시간과 날씨에 따라 자연스럽게 변화**
- 상태가 단순 수치가 아니라 **게임플레이에 실질적 영향** 주도록 구현
- `float` 변수로 상태 수치 관리 (`hunger`, `thirst`, `cold`, `mental` 등)
- `Image.fillAmount`로 상태바 UI 실시간 표시
- `WeatherType` Enum과 `isInColdWeather` 플래그로 추위 판정 처리
- 상태 수치에 따라 **이동 속도 감소, 회복 불가 등 부가 효과 연계**
- 날씨 시스템과 컨디션 시스템을 연결
    
    (예: 폭풍이면 추위가 빠르게 누적되고 스태미나 회복이 중단됨)
    
- 모닥불 근처에서는 자동으로 회복되도록 트리거 조건 설정

---

<br><br>


</details>
<details><summary>몬스터 / 동물 AI 시스템</summary>

### 날씨 시스템
<img src="https://github.com/user-attachments/assets/5f50d5a8-541a-44dc-be41-e69dd640cb63" width="400" height="300">

Weather
- 날씨 상태를 `Sunny`, `Rainy`, `Hot`, `Snow` 네 가지로 설정
- `SetRandomWeather()`: 1~100 사이 난수를 생성해 25% 확률로 각 날씨를 결정
- 게임 시작 시 `Start()`에서 날씨를 랜덤하게 설정

UIWeather
- 날씨 UI 관리
    - `sunny`, `rainy`, `hot`, `snow` 게임 오브젝트를 통해 현재 날씨 상태를 UI로 표시
- 날씨 효과 적용 (`Update`)
    - `Weather` 클래스에서 `currentWeather` 값을 가져와 현재 날씨를 확인
    - 모든 날씨 UI 요소를 초기화한 후 현재 날씨에 맞는 UI를 활성화
    - `rainParticle` 및 `snowParticle`이 존재하면 해당 날씨에서 파티클 효과 재생
- 파티클 위치 조정 (`LateUpdate`)
    - 카메라의 위치를 기준으로 `rainParticle`과 `snowParticle`의 위치를 조정하여 플레이어 이동 시에도 파티클이 따라오도록 설정


---
<br><br>

</details>
<details><summary>인벤토리 시스템</summary>

### 인벤토리리 시스템

<img src="https://github.com/user-attachments/assets/351311c9-6ad8-4cdb-90f3-95a890ca1df7" width="400" height="300"> <br>
인벤토리 시스템 구조:	InventoryBase, InventoryManager를 중심으로 다양한 UI 인벤토리 요소(UIInventory, UIEquip, UIQuickSlot)을 관리
슬롯 시스템:	SlotBase를 상속한 EquipSlot, InventorySlot 등이 있으며, 각 슬롯은 클릭 및 마우스 오버 이벤트를 처리
툴팁 기능:	Tooltip, TooltipController, TooltipDataBuilder 등을 통해 아이템에 마우스를 올리면 설명창 표시
아이템 정보 관리:	ItemData, ItemObject, ItemEffectProcessor를 통해 아이템의 속성, 효과를 정의하고 적용
상호작용 시스템:	Interaction을 통해 카메라 중심에 Raycast로 상호작용 가능한 오브젝트 탐색 및 상호작용 UI 제공
싱글톤 패턴 사용:	InventoryManager는 Singleton 패턴으로 전역 UI 접근 제공


---
<br><br>


---
<br><br>

</details>
