Á
[C:\Users\macie\Source\Repos\WorkStation\WorkstationClient\WorkstationSignalR_Dev\ChatHub.cs
	namespace 	"
WorkstationSignalR_Dev
  
{ 
public		 

class		 
ChatHub		 
:		 
Hub		 
{

 
public 
void 
Send 
( 
string 
name  $
,$ %
string& ,
message- 4
)4 5
{ 	
Clients 
. 
All 
. 
broadcastMessage (
(( )
name) -
,- .
message/ 6
)6 7
;7 8
} 	
} 
} µ
jC:\Users\macie\Source\Repos\WorkStation\WorkstationClient\WorkstationSignalR_Dev\OWINSignalRChatStartup.cs
[ 
assembly 	
:	 

OwinStartup 
( 
typeof 
( "
WorkstationSignalR_Dev 4
.4 5"
OWINSignalRChatStartup5 K
)K L
)L M
]M N
	namespace 	"
WorkstationSignalR_Dev
  
{		 
public

 

class

 "
OWINSignalRChatStartup

 '
{ 
public 
void 
Configuration !
(! "
IAppBuilder" -
app. 1
)1 2
{ 	
app 
. 

MapSignalR 
( 
) 
; 
} 	
} 
} –
kC:\Users\macie\Source\Repos\WorkStation\WorkstationClient\WorkstationSignalR_Dev\Properties\AssemblyInfo.cs
[ 
assembly 	
:	 

AssemblyTitle 
( 
$str 1
)1 2
]2 3
[		 
assembly		 	
:			 

AssemblyDescription		 
(		 
$str		 !
)		! "
]		" #
[

 
assembly

 	
:

	 
!
AssemblyConfiguration

  
(

  !
$str

! #
)

# $
]

$ %
[ 
assembly 	
:	 

AssemblyCompany 
( 
$str 
) 
] 
[ 
assembly 	
:	 

AssemblyProduct 
( 
$str 3
)3 4
]4 5
[ 
assembly 	
:	 

AssemblyCopyright 
( 
$str 0
)0 1
]1 2
[ 
assembly 	
:	 

AssemblyTrademark 
( 
$str 
)  
]  !
[ 
assembly 	
:	 

AssemblyCulture 
( 
$str 
) 
] 
[ 
assembly 	
:	 


ComVisible 
( 
false 
) 
] 
[ 
assembly 	
:	 

Guid 
( 
$str 6
)6 7
]7 8
["" 
assembly"" 	
:""	 

AssemblyVersion"" 
("" 
$str"" $
)""$ %
]""% &
[## 
assembly## 	
:##	 

AssemblyFileVersion## 
(## 
$str## (
)##( )
]##) *