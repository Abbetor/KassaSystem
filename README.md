# Café Bönan – Kassasystem

Ett kassasystem i konsolen för det fiktiva caféet **Café Bönan**. En kassör registrerar köp, ger rabatt, tar betalt kontant eller med kort, skriver ut kvitto och gör dagsavslut (Z-rapport) när caféet stänger.

Sortiment, kvitton, räknare och Z-rapporter sparas till fil och finns kvar mellan körningar.

> Projektet är byggt som en skoluppgift i grupp. Det är **inte** ett certifierat kassaregister och får inte användas i en riktig verksamhet. Se [Begränsningar](#kända-brister-och-begränsningar).

---

## Innehåll

- [Kom igång](#kom-igång)
- [Använda kassan](#använda-kassan)
- [Funktioner](#funktioner)
- [Regler för pengar och moms](#regler-för-pengar-och-moms)
- [Arkitektur](#arkitektur)
- [Projektstruktur](#projektstruktur)
- [Datalagring](#datalagring)
- [Tester](#tester)
- [Tillval och vem som gjort vad](#tillval-och-vem-som-gjort-vad)
- [Avvikelser från specifikationen](#avvikelser-från-specifikationen)
- [Kända brister och begränsningar](#kända-brister-och-begränsningar)
- [Bidra](#bidra)

---

## Kom igång

**Krav:** [.NET SDK](https://dotnet.microsoft.com/download) <!-- TODO: version, t.ex. 8.0 -->

```bash
git clone <!-- TODO: repo-URL -->
cd KassaSystem
dotnet run --project <!-- TODO: sökväg till konsolprojektet -->
```

Vid start frågar programmet efter kassörens namn. Första gången (när ingen sparad data finns) laddas grundsortimentet nedan.

### Grundsortiment

| Kod | Namn | Pris | Moms |
|---|---|---|---|
| KAF1 | Bryggkaffe | 32,00 kr | 12 % |
| KAF2 | Cappuccino | 42,00 kr | 12 % |
| KAF3 | Caffè latte | 45,00 kr | 12 % |
| TE1 | Te | 30,00 kr | 12 % |
| BUL1 | Kanelbulle | 35,00 kr | 12 % |
| SMO1 | Ostfralla | 59,90 kr | 12 % |
| VAT1 | Mineralvatten | 24,50 kr | 12 % |
| BON1 | Kaffebönor 500 g | 149,00 kr | 12 % |
| MUG1 | Termosmugg | 199,00 kr | 25 % |
| TID1 | Dagens tidning | 35,00 kr | 6 % |

---

## Använda kassan

Ett typiskt köp:

1. Slå in produktkoder (t.ex. `kaf1`, `BUL1` – stora/små bokstäver spelar ingen roll).
2. Ändra antal eller ta bort rader vid behov. Totalsumman visas efter varje ändring.
3. Ge eventuellt rabatt (0–50 % på hela köpet).
4. Välj betalsätt – kontant eller kort.
5. Kvittot visas och sparas.

Ett pågående köp kan makuleras innan betalning. Vid stängning gör kassören dagsavslut, som skriver en Z-rapport och startar en ny försäljningsdag.

<!-- TODO: lägg in menyöversikt / skärmdump när UI:t är klart -->

---

## Funktioner

### Grundversion

- **Köp** – produkter läggs till med kod. Samma produkt igen ökar antalet på befintlig rad. Antal per rad är 1–99. Ett köp utan rader kan inte betalas.
- **Rabatt** – procent på hela köpet, heltal 0–50 %, kan ändras fram till betalning.
- **Moms** – räknas per momssats (6, 12, 25 %) och visas med underlag och belopp.
- **Kontant betalning** – öresavrundning till hela kronor, mottaget belopp och växel.
- **Kortbetalning** – exakt belopp, simulerad terminal.
- **Kvitto** – företagsuppgifter, datum/tid, kvittonummer, kassör, rader, rabatt, totalsumma, moms per sats och betalningsuppgifter.
- **Makulering** – avbryter ett köp innan betalning. Får inget kvittonummer men räknas i dagsrapporten.
- **Dagsavslut (Z-rapport)** – kvitton, försäljning, moms per sats, försäljning per betalsätt, öresavrundning och antal makuleringar.
- **Sortiment** – lägga till, ändra pris på och ta bort produkter.
- **Tålig inmatning** – felaktig inmatning ger ett meddelande, aldrig en krasch.

### Tillval

Utöver grundversionen har varje gruppmedlem byggt ett tillval. Se [Tillval och vem som gjort vad](#tillval-och-vem-som-gjort-vad).

---

## Regler för pengar och moms

Det här är reglerna koden följer. De är strikta – ett öre fel på momsen räknas som fel.

### Belopp

- Alla belopp lagras och räknas som `decimal`, aldrig `double` eller `float`.
- Avrundning till öre sker **bort från noll** vid exakt hälften: `Math.Round(x, 2, MidpointRounding.AwayFromZero)`.

### Rabatt

Rabatten fördelas per rad och avrundas till öre per rad:

```
rabatterad radsumma = styckpris × antal × (1 − rabatt/100)
totalsumma          = summan av de rabatterade raderna
```

### Moms

Alla priser är **inklusive moms**. Momsen räknas fram ur priset, per momssats, på summan för satsen – inte per rad och inte per enhet:

```
summa för satsen = summan av rabatterade radsummor med den satsen
moms             = summa för satsen × sats / (100 + sats), avrundat till öre
momsunderlag     = summa för satsen − moms
```

Exempel: sju bryggkaffe à 32,00 kr ger 224,00 × 12/112 = **24,00 kr** i moms (per kopp hade det blivit 24,01 kr).

Belopp räknas alltid fram från raderna när de behövs och sparas aldrig undan, så att de stämmer efter ändrat antal, borttagen rad eller ändrad rabatt.

### Kontant

- Att betala avrundas till hela kronor, 50 öre uppåt (124,49 → 124 kr, 124,50 → 125 kr).
- Skillnaden visas på kvittot som *Öresavrundning* och kan vara negativ.
- Mottaget belopp måste vara minst att betala. Växel = mottaget − att betala.
- Öresavrundningen påverkar **inte** momsen.

### Kvittonummer och Z-rapporter

- Kvittonummer är löpande från 1 och återanvänds eller hoppar aldrig bakåt, inte heller efter omstart.
- Z-rapportnummer är löpande på samma sätt. En Z-rapport kan inte ändras eller tas bort.
- Ett kvitto är en ögonblicksbild – ändras ett pris i sortimentet visar gamla kvitton fortfarande det gamla priset.

---

## Arkitektur

Koden är uppdelad i tre lager:

- **Domän** – reglerna: produkter, köp, moms, betalning, kvitton, dagsavslut. Domänklasserna läser och skriver **aldrig** till `Console`.
- **UI** – all in- och utmatning. Visar köp och kvitton, ställer frågor och validerar svar.
- **Lagring** – läser och skriver data till fil.

Uppdelningen gör att presentationen (konsol, pekskärm, kvittoskrivare) kan bytas ut utan att reglerna påverkas.

### Klassöversikt

<!-- TODO: uppdatera så att tabellen speglar den faktiska koden vid inlämning -->

| Klass | Lager | Ansvar |
|---|---|---|
| `VatRate` | Domän | De tre momssatserna |
| `Product` | Domän | Kod, namn, styckpris, momssats |
| `Catalog` | Domän | Sortimentet: hitta, lägga till, ändra, ta bort |
| `OrderLine` | Domän | En produkt och ett antal |
| `Order` | Domän | Raderna i ett pågående köp och rabatten; räknar fram totalsumma |
| `VatSummary` | Domän | Moms per sats för en mängd rader |
| `Money` | Domän | Avrundning till öre och hela kronor |
| `Payment` | Domän | Resultatet av en betalning: betalsätt, belopp, öresavrundning, mottaget, växel |
| `PaymentMethod` | Domän | Betalsätt (`Cash`, `Card`) |
| `Receipt` | Domän | Ett slutfört, oföränderligt köp med kvittonummer |
| `Sale` | Domän | En försäljning från första rad till kvitto |
| `Register` | Domän | Kassans dag: kvittonummer, dagens kvitton, makuleringar, dagsavslut |
| `ZReport` | Domän | En oföränderlig dagsrapport |
| `ConsoleUi` | UI | All in- och utmatning |
| `CashierApp` | UI | Meny och huvudloop |
| `DataStorage` | Lagring | Sortiment, kvitton, räknare och Z-rapporter till och från fil |

### Klassdiagram

![Klassdiagram](docs/klassdiagram.png) <!-- TODO: lägg till bilden -->

### Designbeslut

De viktigaste besluten – om radsumma och moms sparas eller räknas fram, skillnaden mellan order och kvitto, var betalningen hanteras och var prisreglerna bor – finns motiverade i [docs/design.md](docs/design.md). <!-- TODO -->

---

## Projektstruktur

<!-- TODO: justera efter faktisk struktur -->

```
KassaSystem/
├── src/
│   └── KassaSystem/
│       ├── Domain/      # Regler och domänklasser
│       ├── Ui/          # ConsoleUi, CashierApp
│       └── Storage/     # DataStorage
├── tests/
│   └── KassaSystem.Tests/
├── docs/
│   ├── design.md
│   └── klassdiagram.png
└── README.md
```

---

## Datalagring

<!-- TODO: filnamn, format och plats -->

| Data | Fil |
|---|---|
| Sortiment | `TODO` |
| Kvitton | `TODO` |
| Kvitto- och Z-räknare | `TODO` |
| Z-rapporter | `TODO` |

- **Saknas filerna** startar programmet med grundsortimentet och tomma räknare.
- **Är en fil trasig** säger programmet till i stället för att krascha.

Vill du börja om från noll: stäng programmet och ta bort datafilerna.

---

## Tester

```bash
dotnet test
```

Testerna täcker framför allt pengar och moms, bland annat:

- Sju bryggkaffe ger moms 24,00 kr, inte 24,01 kr
- Öresavrundning: 124,49 → 124 kr, 124,50 → 125 kr
- Rabatt på köp med blandade momssatser

---

## Tillval och vem som gjort vad

Den gemensamma grunden är byggd av hela gruppen. Ovanpå den har var och en byggt ett tillval.

<!-- TODO: fyll i namn och vilka tillval som faktiskt byggts -->

| Tillval | Beskrivning | Ansvarig |
|---|---|---|
| 1. Konsolgrafik | Kvitto som 40 tecken bred remsa med ram, sortiment och köp sida vid sida, Z-rapport som tabell, färger | TODO |
| 2. Betalsätt och delbetalning | Gemensamt gränssnitt för betalsätt; kontant, kort som kan nekas, simulerad Swish; betalning i flera delar | TODO |
| 3. Kampanjer | Kampanjer från konfigurationsfil: X för Y, kombopris med momsfördelning, tidsstyrd rabatt | TODO |
| 4. Lager och inköp | Lagersaldo, avdrag vid betalning, stopp för slutsålda varor, beställningspunkt, inleverans, inköpsförslag | TODO |
| 5. Statistik och export | X-rapport, topplistor, försäljning per timme, snittkvitto, CSV-export | TODO |

<!-- TODO: kort avsnitt per tillval med hur man använder det och eventuella inställningar/filer -->

---

## Avvikelser från specifikationen

<!-- TODO: lista punkter där ni medvetet avvikit från reglerna, eller skriv "Inga". -->

Inga kända avvikelser.

---

## Kända brister och begränsningar

<!-- TODO: lägg till faktiska brister -->

- **Inte ett certifierat kassaregister.** Svenska företag som säljer mot kontanter eller kort behöver normalt ett certifierat kassaregister med kontrollenhet enligt Skatteverkets regler.
- **Simulerade betalningar.** Ingen koppling till riktig kortterminal eller Swish.
- **Filbaserad lagring.** Ingen databas, ingen säkerhetskopiering och ingen synk mellan flera kassor.
- **En kassör åt gången.** Ingen inloggning eller behörighetsstyrning.
- **Inga returer** och ingen delning av notan.

---

## Bidra

Projektet använder ett Git-flöde med skyddade brancher.

| Branch | Roll |
|---|---|
| `main` | Publicerad version. Endast via PR med två godkännanden. |
| `dev` | Utvecklingsbranch och default branch. Endast via PR med ett godkännande. |
| `feature-...` | En per arbetsuppgift, utgår från `dev` och mergas tillbaka till `dev`. |

```bash
git switch dev && git pull
git switch -c feature-mitt-arbete
# ... commita löpande ...
git switch dev && git pull && git switch feature-mitt-arbete && git merge dev
git push -u origin feature-mitt-arbete
# öppna en pull request mot dev
```

**Kodkonventioner:** klasser, metoder och variabler på engelska; allt användaren ser på svenska. Pengar alltid som `decimal`. Ingen `Console` i domänklasserna.
