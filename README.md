# CSGO-Translator
Simple C# CSGO chat translator


Ever wondered what your foreign language speaking teammates where trying to tell you?
Well wonder no more, because CSGO-Translator is here.

CSGO-Translator is a C# .NET WPF based tool that will automatically detect new chat messages, and translate them to English using Google Translate.

It works by reading the console logfile and analysing it for new messages every few seconds.

100% safe to use on official servers!

EXAMPLE OF TRANSLATION:
![](img\demo.PNG)

OPTIONS AVAILABLE:
![](img\demo2.PNG)

SETUP:
1. Download the lastest release (or build it yourself)
2. Start up a game of CSGO
3. Enter the console command: con_logfile "console.log" (or add it to your autoexec.cfg)
4. run CSGO-Translator.exe
5. (Optionally) Change your "Counter-Strike Global Offensive" folder path in the options menu. (default is default path)
6. (Optionally) Change your language in the options menu by (2-char abbreviation. Example: fr,de,en,ru,es) (Default en)
6. Move the window to your second monitor (or just alt + tab)
7. See live translations of your incoming chat messages

FEATURES:
1. Extremely simple setup, you only need to download 1 file and execute 1 command (or add it to your autoexec.cfg)
2. Lots of languages supported (All that are available in Google Translate)
3. 100% safe to use, all it does is read 1 file in the game directory.
4. Will work with all csgo install locations.
5. Automatically checks for new chats, no need to refresh or anything.

CURRENT LIMITATIONS:
3. Might get a timeout from Google Translate under extreme use.
4. Won't display new message when an identical message has been sent recently (to avoid the Google Translate timeout).
5. Won't display chat messages on certain community servers because of different chat structures.
