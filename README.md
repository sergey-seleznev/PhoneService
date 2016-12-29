# PhoneService
The project is a test task solution for a .NET software developer position at https://www.proekspert.ee.  
It took me 4 days to complete the exercise up to the state provided.

## Assignment
The goal is to prepare a client-service solution to display some legacy data in more modern and meaningful format let’s say for monitoring purposes by humans and other services.

## Service
The goal is to prepare an application that periodically queries data from the network, converts said data to a suitable format and serves it up as an API service in JSON format. In order to obtain the source data, one needs to query a “legacy” data interface, located at http://people.proekspert.ee/ak/. The request format for the interface is “data_n.txt” (wherein n is replaced by a number between 1-9). Sample request: http://people.proekspert.ee/ak/data_1.txt

Request into the legacy interface has to be made periodically every 5 seconds, in an ascending order according to the request parameter index (1, 2, 3, …). The generated service should only return actual data from last request from the legacy interface (data from the previous files should not be buffered).

Data is “service parameters“, provided as positional text. ABBB..CDEFFF.., whereas:  
* A: ‘A’ active / ’P’ passive (1 symbol)  
* BBB..: phone number (20 symbols)  
* C: XL-additional service, ‘J’ yes / ’E’ no (1 symbol)  
* D: 1. language (1 symbol) E=Estonian, I=English  
* E: XL-additional service language (1 symbol)  
* FFF...: service end date (8 symbols, format YYYYMMDD)  
* GGG...: service end time (ttmm) (4 symbols)  
* HHH...: XL service activation time 1 (ttmm) (4 symbols)  
* III...: XL service end time (ttmm) (4 symbols)  
* J: override list; ‘K’ in use / ’E’ not in use (1 symbol)  
* 8*KKK..: phone numbers (15 symbols * 8 = 120 symbols)  
* 8*LLL..: names (20 symbols * 8 = 160 symbols)

## Client
The goal is to prepare a web page that displays data from the API service created by you. Client should update its content as service periodically updates its data. The following items should be displayed regarding the service:  

1. Request sequence number  
2. Associated phone number (from parameter B)  
3. Whether the service is active (from parameter A, the rest of the parameters should only be displayed in case the service is active)  
4. When does the service become inactive (from parameters F and G)  
5. Service language (from parameter D)  
6. XL-service state and activity period (from parameters C, E, H, I; activity period and language should only be displayed in case the service is active)  
7. The list of phone numbers and names contained in the override list (J, K, L; the override list should be displayed only in case an override list is in use)  

## Deliverables  
1. Source code of the solutions  
2. Short build guide  
3. Setup guide  
4. Enclosing additional materials is not prohibited  

## Extra marks
* Unit tests for the source code
* Automated tests for the API service
* Documentation for the API service
