# 파일 구조 설계

## 디렉토리 구조

```
Rpg/
├── build.txt
├── description.txt
├── icon.png
├── Rpg.cs
├── Rpg.csproj
│
├── Docs/                           # 문서
│   ├── NEW_RPG_DESIGN_PROMPT.md    # 전체 설계 문서
│   ├── TODO.md                     # 체크리스트
│   ├── FILE_STRUCTURE.md           # 이 파일
│   ├── BALANCE_REFERENCE.md        # 밸런스 수치 참조
│   └── IMPLEMENTATION_GUIDE.md     # 구현 가이드
│
├── Assets/                         # 리소스
│   ├── Textures/
│   │   ├── UI/
│   │   ├── Skills/
│   │   └── Jobs/
│   └── Sounds/
│
├── Localization/                   # 현지화
│   ├── en-US.hjson
│   └── ko-KR.hjson
│
├── Common/                         # 공통 시스템
│   │
│   ├── Core/                       # 핵심 시스템
│   │   ├── RpgConstants.cs         # 밸런스 상수 모음
│   │   ├── RpgEnums.cs             # 열거형 정의
│   │   ├── RpgFormulas.cs          # 계산 공식 모음
│   │   └── RpgUtils.cs             # 유틸리티 함수
│   │
│   ├── Players/                    # 플레이어 관련
│   │   ├── RpgPlayer.cs            # 메인 ModPlayer
│   │   ├── PlayerLevel.cs          # 레벨/경험치 시스템
│   │   ├── PlayerStats.cs          # 스탯 시스템
│   │   ├── PlayerSkills.cs         # 스킬 관리
│   │   └── PlayerSaveData.cs       # 세이브/로드
│   │
│   ├── Systems/                    # 월드 시스템
│   │   ├── RpgWorld.cs             # 메인 ModSystem
│   │   ├── WorldLevel.cs           # 월드 레벨 계산
│   │   ├── BossProgression.cs      # 보스 진행도
│   │   ├── DifficultyScaling.cs    # 난이도 스케일링
│   │   └── ModCompatibility.cs     # 모드 호환성
│   │
│   ├── NPCs/                       # NPC 관련
│   │   ├── RpgGlobalNPC.cs         # 메인 GlobalNPC
│   │   ├── NPCLevel.cs             # NPC 레벨 계산
│   │   ├── NPCScaling.cs           # 스케일링 로직
│   │   ├── BossLevels.cs           # 보스 레벨 매핑
│   │   └── ExperienceGlobalNPC.cs  # 경험치 지급
│   │
│   ├── Projectiles/                # 투사체
│   │   ├── RpgGlobalProjectile.cs  # 투사체 스케일링
│   │   └── SkillProjectiles/       # 스킬 전용 투사체
│   │
│   ├── Jobs/                       # 직업 시스템
│   │   ├── Core/
│   │   │   ├── JobBase.cs          # 직업 기본 클래스
│   │   │   ├── JobDatabase.cs      # 직업 DB
│   │   │   └── JobDefinitions.cs   # 직업 정의
│   │   │
│   │   ├── Novice/
│   │   │   └── NoviceJob.cs
│   │   │
│   │   ├── FirstJobs/              # 1차 직업 (10개)
│   │   │   ├── Pure/               # 순수 계열 (4개)
│   │   │   │   ├── Blademaster.cs
│   │   │   │   ├── Gunslinger.cs
│   │   │   │   ├── Elementalist.cs
│   │   │   │   └── Conjurer.cs
│   │   │   │
│   │   │   ├── Specialized/        # 특화 계열 (2개)
│   │   │   │   ├── Juggernaut.cs
│   │   │   │   └── Mystic.cs
│   │   │   │
│   │   │   └── Hybrid/             # 하이브리드 (4개)
│   │   │       ├── BattleMage.cs
│   │   │       ├── Shadow.cs
│   │   │       ├── Ranger.cs
│   │   │       └── Warden.cs
│   │   │
│   │   ├── SecondJobs/             # 2차 직업 (20개, 추후)
│   │   └── ThirdJobs/              # 3차 직업 (20개, 추후)
│   │
│   ├── Skills/                     # 스킬 시스템
│   │   ├── Core/
│   │   │   ├── SkillBase.cs        # 스킬 기본 클래스
│   │   │   ├── SkillRegistry.cs    # 스킬 등록
│   │   │   ├── SkillTypes.cs       # 스킬 타입 (Active, Passive 등)
│   │   │   └── SkillEffects.cs     # 스킬 효과 처리
│   │   │
│   │   ├── Novice/                 # 노비스 스킬 (7개)
│   │   │   ├── BasicStrike.cs
│   │   │   ├── Dash.cs
│   │   │   ├── FirstAid.cs
│   │   │   ├── Focus.cs
│   │   │   ├── Defense.cs
│   │   │   ├── Vitality.cs
│   │   │   └── KeenEye.cs
│   │   │
│   │   ├── Blademaster/            # 블레이드마스터 스킬 (8개)
│   │   ├── Gunslinger/             # 건슬링거 스킬 (8개)
│   │   └── ...                     # 나머지 직업 스킬
│   │
│   ├── UI/                         # UI 시스템
│   │   ├── Core/
│   │   │   ├── RpgUI.cs            # UI 매니저
│   │   │   ├── UITheme.cs          # UI 테마
│   │   │   └── UIUtils.cs          # UI 유틸
│   │   │
│   │   ├── Elements/               # UI 요소
│   │   │   ├── StatBar.cs
│   │   │   ├── SkillIcon.cs
│   │   │   ├── ProgressBar.cs
│   │   │   └── Button.cs
│   │   │
│   │   └── Windows/                # UI 창
│   │       ├── StatusWindow.cs     # 상태 창
│   │       ├── SkillWindow.cs      # 스킬 창
│   │       ├── StatsWindow.cs      # 스탯 창
│   │       ├── JobWindow.cs        # 직업 창
│   │       └── SkillBar.cs         # 스킬바 (HUD)
│   │
│   ├── Configs/                    # 설정
│   │   ├── RpgConfig.cs            # 클라이언트 설정
│   │   └── RpgServerConfig.cs      # 서버 설정
│   │
│   └── Net/                        # 네트워크 (멀티플레이)
│       ├── Packets/
│       │   ├── LevelUpPacket.cs
│       │   ├── SkillUsePacket.cs
│       │   └── StatChangePacket.cs
│       └── NetHandler.cs
│
└── Content/                        # 컨텐츠 (아이템, 버프 등)
    ├── Items/
    │   ├── JobTokens/              # 전직 아이템
    │   └── Consumables/
    │
    └── Buffs/
        ├── SkillBuffs/             # 스킬 버프
        └── JobBuffs/               # 직업 버프
```

## 파일 역할 설명

### Core (핵심)
- **RpgConstants**: 모든 밸런스 숫자 (레벨 캡, 스탯 배율 등)
- **RpgEnums**: JobTier, SkillType, DamageType 등
- **RpgFormulas**: GetRequiredXP(), CalculateNPCLevel() 등 계산 공식
- **RpgUtils**: 헬퍼 함수, 확장 메서드

### Players
- **RpgPlayer**: 메인 ModPlayer, 모든 플레이어 데이터 통합
- **PlayerLevel**: 레벨, 경험치 관리 (부분 클래스)
- **PlayerStats**: STR, DEX, INT, VIT, LUK 관리
- **PlayerSkills**: 스킬 슬롯, 쿨다운 관리
- **PlayerSaveData**: TagCompound 세이브/로드

### Systems
- **RpgWorld**: 메인 ModSystem, 월드 전역 데이터
- **WorldLevel**: GetPreHardmodeWorldLevel(), GetHardmodeWorldLevel()
- **BossProgression**: CanUnlockFirstJob(), GetProgressionScore()
- **DifficultyScaling**: 난이도별 배율 계산
- **ModCompatibility**: Calamity, Thorium 등 모드 호환

### NPCs
- **RpgGlobalNPC**: 메인 GlobalNPC, NPC 레벨 저장
- **NPCLevel**: CalculateNPCLevel(), GetBiomeLevelCap()
- **NPCScaling**: ApplyLevelScaling(), HP/Damage 증가
- **BossLevels**: 보스별 고정 레벨 매핑
- **ExperienceGlobalNPC**: OnKill에서 경험치 지급

### Jobs
- **JobBase**: 모든 직업의 기본 클래스
- **JobDatabase**: 직업 정보 조회 (GetJob, GetAllJobs)
- **JobDefinitions**: 10개 1차 직업 정의

### Skills
- **SkillBase**: 모든 스킬의 기본 클래스
- **SkillRegistry**: 스킬 등록/조회
- **SkillTypes**: ActiveSkill, PassiveSkill, ToggleSkill 등
- **SkillEffects**: 버프/디버프, 데미지 계산 등

### UI
- **RpgUI**: UI 상태 관리, 토글
- **Elements**: 재사용 가능한 UI 컴포넌트
- **Windows**: 각종 UI 창

## 구현 우선순위

### 1단계 (핵심 기반)
1. RpgConstants.cs
2. RpgEnums.cs
3. RpgFormulas.cs
4. RpgPlayer.cs
5. PlayerLevel.cs

### 2단계 (월드 & NPC)
1. RpgWorld.cs
2. WorldLevel.cs
3. BossProgression.cs
4. RpgGlobalNPC.cs
5. NPCLevel.cs
6. NPCScaling.cs

### 3단계 (경험치)
1. ExperienceGlobalNPC.cs
2. 경험치 밸런스 테스트

### 4단계 (직업)
1. JobBase.cs
2. JobDatabase.cs
3. NoviceJob.cs
4. 1차 직업 10개

### 5단계 (스킬)
1. SkillBase.cs
2. SkillRegistry.cs
3. 노비스 스킬 7개
4. 1차 직업 스킬 80개

### 6단계 (UI)
1. 기본 HUD (레벨, 경험치바)
2. 스킬바
3. 상태 창
4. 스킬 창

### 7단계 (난이도 & 모드)
1. DifficultyScaling.cs
2. ModCompatibility.cs
3. 테스트 & 밸런스

## 코딩 규칙

- **네임스페이스**: `Rpg.Common.XXX`
- **클래스명**: PascalCase
- **필드/변수**: camelCase
- **상수**: UPPER_SNAKE_CASE
- **주석**: 모든 public 멤버에 XML 주석
- **문서화**: 복잡한 로직은 상단에 설명 주석

## 참조 경로

- 기존 코드: `Reference/Rpg-Backup-20260108-132623/`
- 설계 문서: `Docs/NEW_RPG_DESIGN_PROMPT.md`
- 밸런스: `Docs/BALANCE_REFERENCE.md`
