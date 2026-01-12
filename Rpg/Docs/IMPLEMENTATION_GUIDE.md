# 구현 가이드

이 문서는 NEW_RPG_DESIGN_PROMPT.md와 chatgpt prompt.md를 통합하여 실제 구현 시 참조할 가이드입니다.

## 📋 설계 문서 우선순위

1. **NEW_RPG_DESIGN_PROMPT.md** - 기본 설계 골격
2. **chatgpt prompt.md** - 상세 요구사항 및 특수 케이스
3. **BALANCE_REFERENCE.md** - 구체적 수치
4. **이 문서 (IMPLEMENTATION_GUIDE.md)** - 통합 구현 가이드

---

## 🎯 핵심 차이점 및 통합 사항

### 1. 레벨 시스템

#### 레벨 캡 (보스 진행도 기반)

**프리하드모드** (Lv.1-50):
- 기본 캡: Lv.8 (보스 없음)
- 슬라임킹 (King Slime): +2 (Lv.10)
- 크툴루의 눈 (Eye of Cthulhu): +5 (Lv.15)
- 세상을 먹는 자/크툴루의 뇌 (Eater of Worlds/Brain of Cthulhu): +10 (Lv.25)
- 여왕벌 (Queen Bee): +5 (Lv.30)
- 스켈레트론 (Skeletron): +10 (Lv.40)
- 디어클롭스 (Deerclops): +5 (Lv.45)
- 살의 장벽 (Wall of Flesh) 처치 시: **Lv.50 해제** + 하드모드 진입

**하드모드** (Lv.50-120):
- 하드모드 진입: Lv.60
- 여왕 슬라임 (Queen Slime): +5 (Lv.65)
- 기계 보스들 (Mechanical Bosses) 각각: +10씩 (Lv.95)
  - 트윈즈 (The Twins)
  - 디스트로이어 (The Destroyer)
  - 스켈레트론 프라임 (Skeletron Prime)
- 플랜테라 (Plantera): +10 (Lv.105)
- 골렘 (Golem): +5 (Lv.110)
- 듀크 피시론 (Duke Fishron): +5 (Lv.115)
- 빛의 여제 (Empress of Light): +5 (Lv.120)
- 문 로드 (Moon Lord) 처치 시: **레벨 캡 해제 (무제한)**

**포스트 문로드** (Lv.120+):
- 모드 보스마다 +10~20씩 증가
- 이론상 무한대까지 성장 가능

#### 구현 방법
```csharp
public static int GetMaxLevel()
{
    int maxLevel = 8; // 기본
    
    // 프리하드
    if (NPC.downedSlimeKing) maxLevel = Math.Max(maxLevel, 10);
    if (NPC.downedBoss1) maxLevel = Math.Max(maxLevel, 15);
    if (NPC.downedBoss2) maxLevel = Math.Max(maxLevel, 25);
    if (NPC.downedQueenBee) maxLevel = Math.Max(maxLevel, 30);
    if (NPC.downedBoss3) maxLevel = Math.Max(maxLevel, 40);
    if (NPC.downedDeerclops) maxLevel = Math.Max(maxLevel, 45);
    if (Main.hardMode) maxLevel = Math.Max(maxLevel, 60);
    
    // 하드모드
    if (NPC.downedQueenSlime) maxLevel = Math.Max(maxLevel, 65);
    if (NPC.downedMechBoss1) maxLevel += 10;
    if (NPC.downedMechBoss2) maxLevel += 10;
    if (NPC.downedMechBoss3) maxLevel += 10;
    if (NPC.downedPlantBoss) maxLevel += 10;
    if (NPC.downedGolemBoss) maxLevel += 5;
    if (NPC.downedFishron) maxLevel += 5;
    if (NPC.downedEmpressOfLight) maxLevel += 5;
    
    // 문로드 후: 무제한
    if (NPC.downedMoonlord) maxLevel = int.MaxValue;
    
    return maxLevel;
}
```

### 2. 스탯 시스템 (확장)

| 스탯 | 영문명 | 주 효과 | 부 효과 |
|-----|--------|--------|---------|
| **힘** | Strength (STR) | 근접 데미지 +1%/pt | 소량 투척 데미지 증가 |
| **민첩** | Dexterity (DEX) | 원거리 데미지 +1%/pt<br>치명타 +0.5%/10pt | 소량 근접 데미지<br>이동속도 +0.5%/10pt |
| **지능** | Intelligence (INT) | 마법 데미지 +1.5%/pt<br>마나 +5/pt | 마나 재생 +0.2/5pt |
| **정신** | Spirit (SPI) | 소환 데미지 +1%/pt<br>소환 슬롯 +1/20pt | 소량 마법 데미지 |
| **활력** | Vitality (VIT) | HP +5/pt<br>스태미나 +5/2pt | 방어 +2/5pt<br>HP 재생 소량 |
| **운** | Luck (LUK) | 모든 데미지 +0.2%/pt<br>크리티컬 +0.5%/pt | 드롭률 +1%/10pt |

#### 직업별 스탯 효율
```csharp
// 예: 워리어의 STR 투자 시
if (jobType == JobType.Warrior)
{
    strBonus *= 1.5f; // 50% 추가 효율
}
```

### 3. 경험치 시스템 (정밀화)

#### 일반 몬스터 경험치 공식
```csharp
int CalculateBaseXP(NPC npc)
{
    float hpFactor = npc.lifeMax / 100f;
    float defFactor = 1 + (npc.defense / 10f);
    float dmgFactor = 1 + (npc.damage / 25f);
    
    int baseXP = (int)(hpFactor * defFactor * dmgFactor);
    
    // 월드 레벨 보정
    int worldLevel = Main.hardMode ? 
        GetHardmodeWorldLevel() : 
        GetPreHardmodeWorldLevel();
    
    float worldMultiplier = 1 + (worldLevel * 0.1f);
    
    return (int)(baseXP * worldMultiplier);
}
```

#### 보스 경험치
```csharp
int CalculateBossXP(NPC boss)
{
    int bossLevel = GetBossLevel(boss);
    int baseXP = bossLevel * 500;
    
    // 보스는 체력이 높아 경험치 폭발 방지
    // 최대 플레이어 레벨 * 1000 정도로 제한
    int maxXP = GetAveragePlayerLevel() * 1000;
    
    return Math.Min(baseXP, maxXP);
}
```

#### 특수 케이스 처리

**세그먼트형 보스** (Eater of Worlds, The Destroyer):
```csharp
public override void OnKill(NPC npc)
{
    // 세그먼트는 경험치 없음
    if (IsSegment(npc.type) && !IsHead(npc.type))
    {
        return; // 경험치 없음
    }
    
    // 머리가 죽을 때만 전체 경험치 지급
    if (IsHead(npc.type))
    {
        int totalXP = CalculateSegmentedBossXP(npc);
        GiveExperience(totalXP);
    }
}

bool IsSegment(int npcType)
{
    return npcType == NPCID.EaterofWorldsBody ||
           npcType == NPCID.EaterofWorldsTail ||
           npcType == NPCID.TheDestroyerBody ||
           npcType == NPCID.TheDestroyerTail;
}
```

**이벤트 몬스터**:
```csharp
float GetEventXPMultiplier()
{
    if (Main.bloodMoon || Main.eclipse || 
        Main.pumpkinMoon || Main.snowMoon)
    {
        return 0.5f; // 50% 감소
    }
    
    if (Main.invasionType > 0) // 고블린, 해적 등
    {
        return 0.5f;
    }
    
    return 1.0f;
}
```

**동상 소환 몬스터**:
```csharp
public override void OnKill(NPC npc)
{
    // 동상으로 소환된 경우 경험치 0
    if (npc.SpawnedFromStatue)
    {
        return;
    }
    
    // 정상 경험치 계산...
}
```

### 4. 전직 시스템 (3차 전직 포함)

#### 전직 구조
```
노비스 (Lv.1-10)
    ↓ Lv.10
[1차 전직] (Lv.10-50)
├── 워리어 (Warrior)
├── 레인저 (Ranger)
├── 메이지 (Mage)
└── 서머너 (Summoner)
    ↓ Lv.50 or HM 진입
[2차 전직] (Lv.50-120)
├── 워리어 → 나이트 / 버서커
├── 레인저 → 스나이퍼 / 트래퍼
├── 메이지 → 소서러 / 클레릭
└── 서머너 → 비스트마스터 / 네크로맨서
    ↓ Lv.100-120
[3차 전직] (Lv.120+)
├── 나이트 → 가디언 (Guardian)
├── 버서커 → 블러드나이트 (Blood Knight)
├── 소서러 → 아크메이지 (Archmage)
├── 클레릭 → 대주교 (Archbishop)
└── ... (각 2차마다 1개씩)
```

#### 하이브리드 직업 (2차 전직 옵션)
```
워리어 → 팔라딘 (Paladin) - 전사 + 클레릭 혼합
레인저 → 섀도우 (Shadow) - 원거리 + 근접 혼합
메이지 → 스펠블레이드 (Spellblade) - 마법 + 근접 혼합
서머너 → 드루이드 (Druid) - 소환 + 마법 혼합
```

#### 하이브리드 밸런스
```csharp
// 순수 클래스: 한 타입 +20%, 다른 타입 -50%
if (isPureClass)
{
    primaryDamage *= 1.2f;
    otherDamage *= 0.5f;
}

// 하이브리드: 두 타입 각 +10%, 다른 타입 -30%
if (isHybridClass)
{
    primaryDamage1 *= 1.1f;
    primaryDamage2 *= 1.1f;
    otherDamage *= 0.7f;
}
```

### 5. 스킬 시스템

#### 스킬 슬롯 개수
```
1차 전직: 2개 액티브 슬롯
2차 전직: 4개 액티브 슬롯
3차 전직: 6개 액티브 슬롯
```

#### 스킬 포인트 경제 (수정)
```
노비스: 6 SP/레벨 + 마일스톤
1차: 4 SP/레벨 + 마일스톤
2차: 3 SP/레벨 + 마일스톤
3차: 3 SP/레벨 + 마일스톤
```

### 6. 난이도 대응

#### 클래식 ~ 레전더리 차별화
```csharp
float GetDifficultyDamageScaling(int level, bool isBoss)
{
    float levelDiff = level - 1;
    
    if (Main.getGoodWorld) // 레전더리
    {
        return isBoss ? 
            1 + (levelDiff * 0.001f) : // 보스: +0.1%/레벨
            1 + (levelDiff * 0.002f);  // 일반: +0.2%/레벨
    }
    else if (Main.masterMode)
    {
        return isBoss ?
            1 + (levelDiff * 0.002f) : // +0.2%/레벨
            1 + (levelDiff * 0.004f);  // +0.4%/레벨
    }
    else if (Main.expertMode)
    {
        return isBoss ?
            1 + (levelDiff * 0.003f) : // +0.3%/레벨
            1 + (levelDiff * 0.006f);  // +0.6%/레벨
    }
    else // 클래식
    {
        return isBoss ?
            1 + (levelDiff * 0.005f) : // +0.5%/레벨
            1 + (levelDiff * 0.008f);  // +0.8%/레벨
    }
}
```

### 7. 모드 호환성

#### Calamity 모드 대응
```csharp
if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
{
    // Calamity 보스 레벨 매핑
    // Desert Scourge: Lv.28
    // Supreme Calamitas: Lv.120
    
    // Revengeance 모드 감지
    if (IsCalamityRevengeance())
    {
        // 경험치 획득 80%로 감소
        xpMultiplier *= 0.8f;
        
        // 몬스터 스케일링 완화
        scalingMultiplier *= 0.9f;
    }
}
```

#### Thorium 모드 대응
```csharp
if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
{
    // Bard, Healer 클래스 추가
    // Bard → Ranged 취급
    // Healer → Magic 취급
    
    // 또는 전용 2차 직업 분기 추가
}
```

---

## 🔧 구현 우선순위 (수정)

### Phase 1: 핵심 시스템
1. RpgConstants.cs - 모든 상수
2. RpgEnums.cs - 열거형
3. RpgFormulas.cs - 계산 공식
   - GetRequiredXP() - 지수 곡선
   - CalculateMonsterXP() - HP/공격/방어 기반
   - GetMaxLevel() - 보스 진행도 기반
4. RpgPlayer.cs - 플레이어 기본
5. PlayerLevel.cs - 레벨/경험치

### Phase 2: 월드 & NPC
1. RpgWorld.cs - 월드 시스템
2. WorldLevel.cs - 월드 레벨
3. BossProgression.cs - 보스 추적 + 레벨 캡
4. RpgGlobalNPC.cs - NPC 시스템
5. NPCLevel.cs - 레벨 계산
6. NPCScaling.cs - 스케일링
7. ExperienceGlobalNPC.cs - 경험치 지급
   - 세그먼트 보스 처리
   - 이벤트 몬스터 처리
   - 동상 소환 필터

### Phase 3: 직업 시스템
1. JobBase.cs - 기본 클래스
2. JobDatabase.cs - DB
3. 노비스 + 1차 4개
4. 2차 8개 (순수 + 하이브리드)
5. 3차 8~16개

### Phase 4: 스킬 시스템
1. SkillBase.cs
2. SkillRegistry.cs
3. 노비스 7개
4. 1차 각 8개
5. 2차 각 8개
6. 3차 각 6~8개

### Phase 5: 난이도 & 모드
1. DifficultyScaling.cs
2. ModCompatibility.cs - Calamity/Thorium
3. 밸런스 테스트

---

## 📝 구현 시 주의사항

### 1. 레벨 캡 체크
```csharp
public void GainExperience(int amount)
{
    int maxLevel = GetMaxLevel();
    
    if (Level >= maxLevel)
    {
        // 99.9%에서 정지
        int requiredXP = GetRequiredXP(Level);
        int cappedXP = (int)(requiredXP * 0.999f);
        
        if (TotalExperience >= cappedXP)
        {
            TotalExperience = cappedXP;
            return;
        }
    }
    
    TotalExperience += amount;
    CheckLevelUp();
}
```

### 2. 경험치 분배 (멀티플레이)
```csharp
public void DistributeXP(int xp, Vector2 position)
{
    const float SHARE_RADIUS = 5000f; // 타일 기준
    
    for (int i = 0; i < Main.maxPlayers; i++)
    {
        Player player = Main.player[i];
        if (!player.active) continue;
        
        float distance = Vector2.Distance(player.Center, position);
        if (distance <= SHARE_RADIUS * 16f) // 픽셀 변환
        {
            player.GetModPlayer<RpgPlayer>().GainExperience(xp);
        }
    }
}
```

### 3. 스탯 초기화
```csharp
// 초반 10레벨까지 무료
// 이후 특수 아이템 필요
public bool ResetStats()
{
    if (Level <= 10)
    {
        // 무료 리셋
        ResetAllStats();
        return true;
    }
    
    // "스탯 초기화 물약" 아이템 체크
    if (HasResetPotion())
    {
        ConsumeResetPotion();
        ResetAllStats();
        return true;
    }
    
    return false;
}
```

### 4. UI 메시지
```csharp
// 경험치 획득 메시지 (옵션)
if (ModContent.GetInstance<RpgConfig>().ShowXPGain)
{
    CombatText.NewText(npc.Hitbox, Color.Gold, $"+{xp} XP");
}

// 레벨업 메시지
if (leveledUp)
{
    Main.NewText($"Level Up! Lv.{newLevel}", Color.Gold);
    
    // 레벨 캡 도달 시
    if (newLevel >= GetMaxLevel())
    {
        Main.NewText("Level cap reached! Defeat bosses to increase.", Color.Orange);
    }
}
```

---

## 🎨 UI 요구사항

### HUD 요소
1. **경험치 바** - 화면 하단, 다음 레벨까지 진행도
2. **레벨 표시** - "Lv.45 / 50" (현재/최대)
3. **월드 레벨** - "World Lv.12"
4. **스킬 슬롯** - 10개, 쿨다운 표시
5. **스탯 창** - 포인트 배분 UI

### 색상 체계
- 경험치 바: 골드 (#FFD700)
- 레벨 표시: 흰색
- 레벨 캡 도달: 주황색
- 몬스터 레벨: 위험도별 색상

---

## 📊 테스트 체크리스트

- [ ] 보스 처치 시 레벨 캡 상승 확인
- [ ] 세그먼트 보스 경험치 한 번만 지급
- [ ] 이벤트 몬스터 50% 감소 확인
- [ ] 동상 소환 몬스터 경험치 없음
- [ ] 난이도별 스케일링 차이 확인
- [ ] Calamity/Thorium 모드와 충돌 없음
- [ ] 멀티플레이 경험치 공유 정상 작동
- [ ] 3차 전직 레벨 요구사항 확인

---

이 가이드를 기반으로 단계별 구현을 진행하면 됩니다!
