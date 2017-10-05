Ç
jC:\Users\macie\Source\Repos\WorkStation\WorkstationServices\WorkstationCommon\Company\Infos\CompanyInfo.cs
	namespace		 	
Workstation		
 
.		 
Company		 
.		 
Infos		 #
{

 
public 

static 
class 
CompanyInfoUtil '
{ 
public 
static 
CompanyInfo !
CompanyClaims" /
;/ 0
static 
CompanyInfoUtil 
( 
)  
{ 	
} 	
public 
static 
void 
LoadCompanyInfos +
(+ ,
String, 2
filename3 ;
); <
{ 	
using 
( 
StreamReader 
sr  "
=# $
new% (
StreamReader) 5
(5 6
filename6 >
)> ?
)? @
CompanyClaims 
= 
JsonConvert  +
.+ ,
DeserializeObject, =
<= >
CompanyInfo> I
>I J
(J K
srK M
.M N
	ReadToEndN W
(W X
)X Y
)Y Z
;Z [
} 	
} 
public 

class 
CompanyInfo 
{ 
public 
String 
[ 
] 
ValidAudiences &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
public 
String 
[ 
] 
ValidIssuers $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 

Dictionary 
< 
String  
,  !
String" (
>( )
Claims* 0
{1 2
get3 6
;6 7
set8 ;
;; <
}= >
}   
}!! “
hC:\Users\macie\Source\Repos\WorkStation\WorkstationServices\WorkstationCommon\Properties\AssemblyInfo.cs
[ 
assembly 	
:	 

AssemblyTitle 
( 
$str ,
), -
]- .
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
$str .
). /
]/ 0
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
[## 
assembly## 	
:##	 

AssemblyVersion## 
(## 
$str## $
)##$ %
]##% &
[$$ 
assembly$$ 	
:$$	 

AssemblyFileVersion$$ 
($$ 
$str$$ (
)$$( )
]$$) *