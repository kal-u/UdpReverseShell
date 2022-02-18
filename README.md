# UdpReverseShell
Un reverse shell simple en UDP

Ce script est une preuve de concept pour le contournement des antivirus.
La connexion est établie sur le port 53 en UDP (DNS) afin de ne pas être détecté.
Un pop-up du firewall apparait toutefois mais ne perturbe pas le fonctionnement du reverse shell 

Pour récupérer le reverse shell, on lance netcat avec la commande suivante :
'sudo nc -ulvp 53
