BearBones-Messaging
===================

<img src="https://github.com/i-e-b/BearBones-Messaging/raw/master/bonebear.png" width="169" height="184"/>

BearBones messaging: lower-level framework, part of a contract-interface based distributed event framework for .Net


Rough road-map
--------------

* Capture and handle dead messages (i.e. no one is handling a message type) -- maybe by having a failed send on the sender side and a special general dead messages queue (rather than error queue for each real queue)
* Temporary listening -- delete consumer queue when no longer listening.
