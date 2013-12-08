accord-net-extensions
=====================

Extension library for Accord.NET

adds support for:

      generic image 
      easy color converting e.g. img.Convert<Hsv, byte>() (parallel)
      parallel image processing (many methods are written to utilize multicore CPUs)
      optimized convolution (parallel spatial and parallel in frequency domain - FFT)
      introduces new extension and compatibility methods (for Bitmap, UnmanagedImage...)
      introduces extension pattern method usage (e.g. img.GaussianSmooth(3)...) (parallel)
      implements new algorithms (Camshift (parallel), KLT (parallel), ParticleFilter, KalmanFilter)
   
   
roadmap: //TODO list
	  
	implement missing overloads
	write extension methods for existing classes/filters
	overall look and feel
	merge it to Accord.NET
