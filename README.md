# Robocon 2026 Simulation

ABU Robocon 2026 тэмцээний стратеги боловсруулах, тестлэх зориулалт бүхий Unity симуляци. KFS (Korean Flag Symbol) байрлуулах, Мэйхуа зам туулах, Tic-tac-toe самбар дээр байрлал эзэлх зэрэг тэмцээний үндсэн механикуудыг дуурайлган гүйцэтгэнэ.

Симуляци нь **[Robocon2026-ML](Robocon2026-ML/)** sub-repo-той хамт ажилладаг: Unity симуляци датасет үүсгэж, ML sub-repo тухайн датасет дээр YOLO загвар сургаж, дараа нь HTTP сервераар таамаглал буцааж роботод дамжуулна.

---

## Репозиторийн бүтэц

```
Robocon2026Simulation/   ← Unity симуляци (энэ repo)
└── Robocon2026-ML/      ← ML sub-repo (git submodule)
    ├── train.py         ← YOLO загвар сургах
    ├── predict-http.py  ← HTTP inference сервер (port 3445)
    ├── data.yaml        ← 65 классын датасет тохиргоо
    └── datasets/        ← Unity-с үүссэн зургууд (автоматаар үүснэ)
```

---

## Ашигласан технологи

### Unity симуляци

| Технологи | Хувилбар |
|---|---|
| Unity Editor | 6000.2.10f1 |
| Render Pipeline | HDRP 17.2.0 |
| Input System | 1.14.2 |
| UI / TextMeshPro | uGUI 2.0.0 |
| Visual Scripting | 1.9.8 |
| Хэл | C# (.NET) |

### ML sub-repo ([Robocon2026-ML](Robocon2026-ML/))

| Технологи | Зориулалт |
|---|---|
| Python 3 | Үндсэн хэл |
| Ultralytics YOLO | Объект илрүүлэлт (65 класс) |
| Pillow / OpenCV | Зураг боловсруулалт |
| HTTP сервер | Unity-с зураг хүлээн авч таамаглал буцаах |

---

## Суулгах болон ажиллуулах заавар

### Шаардлага

- Unity Hub суулгасан байх
- Unity **6000.2.10f1** хувилбар (Hub-аар суулгана)
- Python **3.10+** (ML sub-repo-д)
- Linux x86\_64 эсвэл Windows платформ

### 1. Clone (submodule-тай хамт)

```bash
git clone --recurse-submodules https://github.com/molor824/Robocon2026Simulation.git
```

Хэрэв аль хэдийн clone хийсэн бол:

```bash
git submodule update --init --recursive
```

### 2. ML сервер тохируулах

```bash
cd Robocon2026-ML
pip install ultralytics pillow
python predict-http.py
```

Сервер `http://localhost:3445` дээр эхэлнэ. Unity Camera stream энэ хаягт зураг илгээж таамаглал авна.

> Загвар сургахын тулд эхлээд Unity PlayScene-д датасет үүсгэж, дараа нь `python train.py` ажиллуулна.

### 3. Unity симуляци ажиллуулах

1. Unity Hub → **Open** → clone хийсэн үндсэн хавтасыг сонгоно.
2. Unity шаардлагатай пакетуудыг автоматаар татна.
3. `Assets/PlayScene.unity` дүрслэлийг нээнэ.
4. **Play** дарж симуляцийг эхлүүлнэ.

---

## Үндсэн функцууд

### KFS сонголтын систем
Өрсөлдөгчийн талбар дээрх жинхэнэ, хуурамч болон R1 KFS дүрсийг ялган таних классификацийн индекс систем (`Kfs`, `KfsSpawner`, `KfsSelection`).

### R2 роботын автомат шийдвэр гаргалт
R2 робот Мэйхуа замаар зөв гарц сонгон KFS авч, дараа нь Tic-tac-toe самбарт байрлуулах бүрэн автомат логик (`R2DecisionMaking`, `R2Movement`, `R2GrabKfs`).

### Мэйхуа навигаци
Мэйхуа зам дахь KFS байрлалыг шинжлэн гурван стратегийн аль тохиромжтойг динамикаар сонгоно (дунд / зүүн / баруун зам).

### Tic-tac-toe самбар
9 нүдтэй самбар дахь KFS байрлуулах дарааллыг удирдах (`TicTacToe`, `TicTacCell`).

### Camera stream & Dataset generation
Unity-н камераас PNG зургийг `http://127.0.0.1:3445` рүү POST хүсэлтээр дамжуулж, `Robocon2026-ML/predict-http.py` серверээс класс + bbox хариу авна. `DatasetGenerator` нь YOLO форматтай `.txt` label болон `.png` зураг хосоор үүсгэж хадгална (`CameraStream`, `DatasetGenerator`, `LabelGenerator`).

### Тоглоомын төлөв удирдлага
3 минутын хугацааны тоолуур, дахин эхлүүлэх болон дуусгах логик, KFS тохируулгын дараалал (`GameStateManager`).

### Эхний биеийн харагч (Spectator)
WASD + хулгана удирдлагатай чөлөөт камер, түр зогсоох горим (`Movement`).

---

## Гишүүд

| Нэр | Оюутны дугаар |
|---|---|
| М.Молор | s21c019b |

---

## Лицензи

[LICENSE](LICENSE) файлыг үзнэ үү.
