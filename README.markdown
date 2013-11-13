Pointer Performance Test
========================

Some quick and dirty C# code to evaluate the performance difference between
normal array access in managed code and unsafe pointer access.

Results
-------

    Results from a build run outside of visual studio, using an
    Any CPU | Release build on the following pc specification:
    
    x64-based pc, 1 quad-core processor
    intel64 family 6 model 23 stepping 10 genuineintel ~2833 mhz

    linear array access
     00:00:07.1053664 for normal
     00:00:07.1197401 for unsafe
     
    linear array access - with pointer increment
     00:00:07.1174493 for normal
     00:00:10.0015947 for unsafe (*p++)
     
    random array access
     00:00:42.5559436 for normal
     00:00:40.5632554 for unsafe
     
    random array access using parallel.for(), with 4 processors
     00:00:10.6896303 for normal
     00:00:10.1858376 for unsafe

License
-------

MIT permissive license. See MIT-LICENSE.txt for full license details.     
     
Source Code Repository
----------------------
 
https://github.com/thomasbratt/PointerPerformanceTest


[![Bitdeli Badge](https://d2weczhvl823v0.cloudfront.net/thomasbratt/pointerperformancetest/trend.png)](https://bitdeli.com/free "Bitdeli Badge")

