# SisProgProjekat1
 First project for system programming course done in pair with colleague Mladen Agic

Task:
Kreirati Web server koji klijentu omogućava pretragu umetničkih dela korišćenjem Art Institute
of Chicago API-a. Pretraga se može vršiti pomoću filtera koji se definišu u okviru query-a. Spisak
umetničkih dela koje zadovoljavaju uslov se vraćaju kao odgovor klijentu (pretragu vršiti po
autoru ili korišćenjem Full Text search opcije). Svi zahtevi serveru se šalju preko browser-a
korišćenjem GET metode. Ukoliko navedena umetnička dela ne postoje, prikazati grešku klijentu.
Način funkcionisanja Art Institute of Chicago API-a je moguće proučiti na sledećem linku:
https://api.artic.edu/docs/#introduction 
Primer poziva serveru: https://api.artic.edu/api/v1/artworks/search?q=cats
