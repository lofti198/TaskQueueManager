# TaskQueueManager
TaskQueueManager is a part of microservice based system called Valid Loader. The system purpose is to return user validly loaded website pages (actually - webpage source codes). "Validly loaded website pages" stand for loaded without captcha. 

Basic logic: 
1. getting new task from the user, containing webpage URL
2. loading webpage using own proxy database (which user doesnt have access to)
3. return result to user

System contains of 2 microservices. The first one (called TaskQueueManager) is responsible for interaction with client application (getting tasks from client and returning results being asynchronously polled by user for readiness of the task). The second one WebpageLoader is directly responsible for actual client task execution.

Step by step breakdown:
1. client sending new task (with particular URL to load specified) to the TaskQueueManager. TaskQueueManager in turn assign task a unique ID (further TID) and returns it to the Client. Now client is supposed to wait for task readiness using Asynchronous Polling approach (and the task being waited periodically polling statusUrl with TID as a parameter).
2. TaskQueueManager publish new task to RabbitMQ queue (further TaskQueue). It contains task ID, the URL to load.
3. The second microservice (further WebpageLoader) is subscribed for new tasks in TaskQueue. When new tasks arrives, it takes it, processes. Note that there supposed to be several instances of WebpageLoaders working simultaneously (distributed through several maschines). 
4. When WebpageLoader finished processing the task successfully, it puts the content of loaded webpage to the separate MSSQL database table (Results) also marking it with TID (of original task).  
5. Now Client which is continuously checking statusUrl (considering Asynchronous Polling) getting the result of task completing .

Also there is another separate project VLUserPanel  (ASP .NET Core MVC), which is a user panel, where users can register and get their API keys. Api keys are used by users to enter in Client application, since TaskQueueManager (which Client app requests) are protected by API keys (using action filter middleware).