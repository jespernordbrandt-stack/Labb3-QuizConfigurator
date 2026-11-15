\# Labb 3 \\- Quiz Configurator



\## Beskrivning



Detta projekt är en quizapplikation byggd i WPF enligt MVVM\\-arkitektur (Model\\-View\\-ViewModel).



Användaren kan skapa och redigera quiz\\-paket, lägga till och ta bort frågor, konfigurera svårighetsgrad och tidsgräns samt spela quiz direkt i applikationen.



Quiz\\-paket sparas och läses in asynkront via JSON\\-filer i användarens AppData\\-mapp.



För VG\\-delen implementerades även import av frågor från Open Trivia Database via ett externt API.



\## Viktiga tekniska lösningar



\### MVVM



Applikationen är uppbyggd enligt MVVM där:



\- Models innehåller data

\- ViewModels innehåller logik

\- Views innehåller XAML och presentation



Commands används för användarinteraktion istället för code\\-behind där det är möjligt.



\### JSON\\-hantering



Quiz\\-paket sparas och läses in asynkront med hjälp av JsonSerializer.



Data lagras i en JSON\\-fil under användarens AppData\\\\Local\\-mapp så att quiz\\-paketen finns kvar mellan programstarter.



\### Open Trivia Database



Frågor kan importeras från Open Trivia Database.



Användaren kan välja:



\- Kategori

\- Svårighetsgrad

\- Antal frågor



API\\-anrop görs asynkront så att användargränssnittet inte låses medan data hämtas.



Felhantering finns för både nätverksproblem och misslyckade API\\-anrop.



\### Slumpning av frågor och svar



Frågornas ordning slumpas varje gång ett quiz startas.



Svarsalternativen slumpas också för varje fråga så att det korrekta svaret inte alltid visas på samma plats.



\## Problem och lösningar



\### Problem 1 \\- Svårighetsgrad importerades inte korrekt



När frågor importerades från Open Trivia Database fungerade inte valet av svårighetsgrad.



Efter felsökning upptäcktes att ComboBoxen i importdialogen var kopplad till ComboBoxItem\\-objekt istället för enum\\-värden.



Lösningen blev att låta ComboBoxen använda enum\\-värden direkt via databinding till Difficulty, vilket gjorde att rätt svårighetsgrad skickades till API:et.



\### Problem 2 \\- HTML\\-kodade tecken i frågor



Vissa importerade frågor innehöll tecken som:



\- "

\- '

\- \&



Problemet berodde på att Open Trivia Database returnerar HTML\\-kodad text.



Lösningen blev att använda WebUtility.HtmlDecode() innan frågorna sparades i programmet.



\### Problem 3 \\- Ogiltiga quiz\\-paket vid misslyckad import



Om API\\-anropet misslyckades kunde tomma quiz\\-paket ibland skapas.



Lösningen blev att avbryta importen om inga frågor returnerades från API:et, vilket förhindrar att ogiltiga paket läggs till i programmet.



\## Tekniker som använts



\- C\\#

\- WPF

\- XAML

\- MVVM

\- Data Binding

\- Commands

\- JsonSerializer

\- Async/Await

\- Open Trivia Database API

\- Git och GitHub





