# Proiect2_MPS_Typing_Game
Second project for the Management of Software Projects Course
Team members:

Adelina Chelcea
Alexandru Bottea
Bogdan-Lucian Basuc 
Adrian Salomia
Dragos Iancu
Vlad Ursu

Current status:

1. Am generat un fisier cu mai multe cuvinte aleatoare in engleza(12kB mai exact). 
Ca sa ne fie mai usor cu partea asta am zis sa avem doar un fisier si la fiecare nivel sa difere 2 lucruri:

  1.1. Timpul per litera (Fiecare cuvant are timpul setat in functie de numarul de litere. Astfel la easy fiecare litera are 0.95s, la medium 0.75s iar la hard 0.6s.)
  
  1.2. Animatia
  
		1.2.1 Easy -> la easy o sa fie text simplu sau animatii simple precum text colorat, scalat, rotit sau cu litere diferentiate
    
		1.2.2 Medium -> la medium o sa fie text normal cu schimbarea culorii fundalului, scalat si colorat, litere diferentiate si colorate, scalate si rotite, litere diferentiate si rotite.
    
		1.2.3 Hard -> la hard o sa fie translatii si rotiri constante in frame-ul jocului (random), translatari constante a literelor diferentiate, rotiri constante ale textului colorat, rotiri si translatii constante ale textului scalat, dar si text care apare si dispare la diferite momente de timp de pe ecran.
    
2. GUI.
Am realizat un GUI de start cu optiunea de start joc sau show leaderboard. Daca intra in starea de vizualizat leaderboard-ul se vor citi dintr-un fisier combinatii de cate 2 linii (nume & scor) si se vor afisa pe ecran. Acel fisier ar trebui sa tina doar primele 10 persoane.
Daca intra in starea de start game vei fi indrumat sa-ti inserezi numele si apoi incepe jocul. Sunt listeneri adaugati in mare parte pe enter(treci mai departe) si esc(iesi din leaderboard).
Cand incepe jocul se vor afisa mereu pe ecran vietile pe care le mai ai, alaturi de scor si timp.

3. Scor
Am implementet si o mica logica a scorului si anume, daca inserezi in ultima secunda cuvantul vei primi 2 puncte in loc de unul si scorul afisat se va face verde.
Si daca ai jucat odata jocul si intri pe leaderboard vei avea toate inregistrarile din leaderboard trecute cu rosu.

TODO list:

1. Customizari (background de start, butoate mai bine aranjate, muzica de fundal, efecte sonore cand pierzi o viata sau iei bonus)

2. Logica scorului (mai pot fi adaugate optiuni pentru a obtine bonus de scor sau puncte mai multe la nivele mai grele)

3. Leaderboard-ul (nu am implementat inca adaugarea in leaderboard (mai exact inserarea numelui si scorului in cei 2 vectori si sortati apoi taiati pana raman doar 10 elemente si rescrierea lor in fisier)


