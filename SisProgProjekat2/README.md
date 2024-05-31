# SisProgProjekat2
 Second project for system programming course done in pair with colleague Mladen Agic

Task (Project 1):
Kreirati Web server koji klijentu omogućava pretragu umetničkih dela korišćenjem Art Institute
of Chicago API-a. Pretraga se može vršiti pomoću filtera koji se definišu u okviru query-a. Spisak
umetničkih dela koje zadovoljavaju uslov se vraćaju kao odgovor klijentu (pretragu vršiti po
autoru ili korišćenjem Full Text search opcije). Svi zahtevi serveru se šalju preko browser-a
korišćenjem GET metode. Ukoliko navedena umetnička dela ne postoje, prikazati grešku klijentu.
Način funkcionisanja Art Institute of Chicago API-a je moguće proučiti na sledećem linku:
https://api.artic.edu/docs/#introduction 
Primer poziva serveru: https://api.artic.edu/api/v1/artworks/search?q=cats

Task (Project 2)

Za drugi projekat timovi treba da implementiraju isti zadatak koji su imali za prvi projekat, uz izmenu da sada treba koristiti taskove i asinhrone operacije (tamo gde to ima smisla). 
Za obradu kod koje taskovi nemaju smisla treba zadržati klasične niti. Dozvoljeno je korišćenje mehanizama za međusobno zaključavanje i sinhronizaciju.
