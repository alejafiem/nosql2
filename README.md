# Moje Dane

Do zadania posłużyłem się policyjnymi danymi incydentów dostarczanymi przez miasto San Francisco, możemy je znaleźć [tutaj](https://data.sfgov.org/Public-Safety/-Change-Notice-Police-Department-Incidents/tmnf-yvry)

## Specyfikacja
OS: Windows 10 64bit
CPU: Intel Core i5-3470 CPU @ 3.20GHz
Ilość rdzeni: 4
RAM: 8.00 GB

## Serwer
Serwery standalone oraz replica uruchamiamy lokalnie korzystając kolejno ze skryptów [standalone_server.bat](standalone_server.bat) oraz [replica_server.bat](replica_server.bat)

## Skrypty
Chcąc wygenerować wyniki musimy uruchomić script.exe tak jak na poniższym obrazku.

![alt text](https://i.imgur.com/N0XhVG1.png)

Skrypt wykona 5 importów dla bazy standalone oraz 5 importów do replica set według poniższej listy:

* default
* w: 1, j: false
* w: 1, j: true
* w: 2, j: false
* w: 2, j: true

Dla każdego importu wyniki generowane są kolejno w plikach:

* [standalone.md](standalone.md)
* [replica_default.md](replica_default.md)
* [replica_w1jf.md](replica_w1jf.md)
* [replica_w1jt.md](replica_w1jt.md)
* [replica_w2jf.md](replica_w2jf.md)
* [replica_w2jt.md](replica_w2jt.md)

## Wyniki
Wyniki każdej z opracji są dostępne w powyższych plikach. Dodatkowo naniosłem dane na wykres:

![alt text](https://i.imgur.com/q1xzHBm.png)

Zaskakujące są dane importu do replica setu, wszystkie wyniki są bardzo zbliżone - ciężko mi powiedzieć dlaczego tak jest, podejrzewam, że jest to spowodowane zbyt mała ilością danych lub przeoczeniem w konfiguracji środowiska. Z ciekawszych rzeczy warto wspomnieć, że przy replica_w2jf oraz replica_w2jt musiałem nastawić wtimeout na wartość 0, aby wyłaczyć możliwość blokowania się zapisu, w przeciwnym razie dostawałem timeout.

# Projekt
Do wykoniania zadania użyłem MongoDB driver dla języka c#. Aplikację uruchamia się z linii poleceń. Standardowo server replica odpalamy przez skrypt [replica_server.bat](replica_server.bat), a dane wczytujemy za pomocą [import_replica.bat](import_replica.bat). 

Mamy do dyspozycji 3 komendy:

`.\zaliczenie.exe daysofweek`

Po wpisaniu tego polecenia skrypt generuje dane na temat ilości przestępstw w każdym z dni tygodnia, następnie oblicza procent udziału przestępstw w danym dniu na tle całego tygodnia i zrzuca te dane do [daysofweek.md](daysofweek.md), dodatkowo tworzy wykres kołowy z tymi danymi - plik [chart.png](chart.png).

`.\zaliczenie.exe index`

W tym miejscu wywołuję proste zapytanie o posortowane elementy. Następnie wykonuje ponowanie te same zapytanie z założonym indeksem na sortowane pole. Wynik czasu zapytania wyświetlam w konsoli tak jak na screenie poniżej.

![alt text](https://i.imgur.com/IISNDTM.png)

`.\zaliczenie.exe index`
Ostatnie polecenie wyświetla incydenty, które miały miejsce w okolicy Sądu San Fransisco (San Fransisco Hall of Justice). Zapytanie te jest wykonywane za pomocą regexu i jest limitowane do 1000 dokumentów. Po ich znalazieniu sa one zrzucane do [justice.json](justice.json).
