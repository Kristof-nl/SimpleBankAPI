# SimpleBankAPI

Project made in .Net 5

To register please write in the field "roles": ["User"]

Live demo: https://simplebankapiswegger.azurewebsites.net/index.html

Project with authentication and authorization. Without login user can only see some actions:  
/api/BankAccount/GetAll  
/api/BankAccount/GetPagedList  
/api/BankAccount/Filter  
/api/Transaction/GetAll  
/api/Transaction/GetPagedList  
/api/Transaction/Filter

After successfully login user get access to few more actions:  
/api/BankAccount/GetById/{id}  
/api/BankAccount/Transfer  
/api/Transaction/GetById/{id}  
/api/BankAccount/Withdraw  

Rest of the actions can be administered only by the Admin.
