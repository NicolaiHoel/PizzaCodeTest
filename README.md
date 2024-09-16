Tanker og antakelser

Antar at man kan få LoyaltyMembership gjennom egne avtaler som ikke krever 3 CompletedOrders

Antar at antall orders aldri kan være negative


Hva ville jeg gjort med mer tid/veien videre:

Legge til et kø system, enten oppå TCP-bindingen eller istedenfor. F.eks Kafka eller Azure Eventhub

Legge til en database for permantent lagring og sett på muligheten for å kvitte meg med in-memory Dictionary.

Skrevet TESTER. Viktig med dekkende enhetstester for systemet.

Implementere Features:

Det ble lagt til et nytt felt i Eventet for å ta i mot ekstra data. Planen er å
 - Bruke den datan til å lagre informasjon om PizzaTyper og CustomerFeedback
 - Bruke PizzaPrepared event til "Inventory Managment" og en liste med Order-obejcts med tilhørende 
 - Laget Orders som et object med informasjon om PizzaType.