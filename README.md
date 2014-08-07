# TickedPriorityQueue

A small library for .NET, giving access to priority based ticking of any number of objects.

It load balances the invocation of the tick events for object event handler, with a user definable maximum limit for objects to be processed in a single update. 

Originally created by [Michael Garforth](https://github.com/Garufortho/) and currently maintained by [Ricardo J. Méndez](http://www.linkedin.com/in/ricardojmendez). Pull requests are welcome.

# Usage

You’ll need to build TickedPriorityQueue.dll from the included solution.  If you plan to use it with Unity, [UnityTickedQueue ](https://github.com/ricardojmendez/TickedPriorityQueue/blob/master/UnityTickedQueue/UnityTickedQueue.cs) is a singleton that will simplify your life.

See the [unit tests](https://github.com/ricardojmendez/TickedPriorityQueue/tree/master/TickedPriorityQueue%20Unit%20Tests/Source/Unit%20Tests) for use examples.

# License

TickedPriorityQueue is open source software released under the [MIT license](https://en.wikipedia.org/wiki/MIT_License). See LICENSE.txt for more details.
