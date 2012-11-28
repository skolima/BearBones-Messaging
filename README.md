BearBones-Messaging
===================
Things it should do
-------------------

* Everything it currently does.
* Capture and handle dead messages (i.e. no one is handling a message type) -- maybe by having a failed send on the sender side and a special general dead messages queue (rather than error queue for each real queue)
* Health Check & re-establish connections (when listening, if RMQ goes down, should reconnect and listen when it's back)
* Temporary listening -- delete consumer queue when no longer listening.
* Should be able to query how many handlers are currently running (interlocked incr/decr?)
* Should be able to set a service to 'cool down' mode where handlers are deregistered and subscriptions stopped (this with counting will help cold code swapping)
* Disconnect and die method (unreg all handlers)
* Change name!

Cool code swap
--------------
Assuming that supervisord or similar is watching and restarting service processes, should be able to support this workflow:

1. service is running, using message handling registration as daemon keep-alive
2. ssh/git deploy new version of software, **without** doing a supervisorctl restart
3. send ICodeSwap message with name of target service
4. target service responds by calling messaging disconnect-and-die on itself
5. all currently running message handlers keep running until they exit normally (any subsequent ICodeSwap events would stack up)
6. service quits
7. supervisord restarts the service (and if it can't start back up, supervisord can have a roll-back policy)
8. service comes back up (?and wipes it's ICodeSwap messages?)

Event store/ event sourcing
---------------------------
* Some way of doing journal & write phases?
* Failure marking on messages, ability to hook in a retry listener?
* Clocks / vectoring?