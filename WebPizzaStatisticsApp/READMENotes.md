Tanker og antakelser

Antar at man kan f� LoyaltyMembership gjennom egne avtaler som ikke krever 3 CompletedOrders

Antar at antall orders aldri kan v�re negative


Hva ville jeg gjort med mer tid/veien videre:

Legge til et k� system, enten opp� TCP-bindingen eller istedenfor. F.eks Kafka eller Azure Eventhub

Legge til en database for permantent lagring og sett p� muligheten for � kvitte meg med in-memory Dictionary.

Skrevet TESTER. Viktig med dekkende enhetstester for systemet.

Implementere Features:

Det ble lagt til et nytt felt i Eventet for � ta i mot ekstra data. Planen er �
 - Bruke den datan til � lagre informasjon om PizzaTyper og CustomerFeedback
 - Bruke PizzaPrepared event til "Inventory Managment" og en liste med Order-obejcts med tilh�rende 
 - Laget Orders som et object med informasjon om PizzaType.