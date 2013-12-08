accord-net-extensions
=====================

Extension library for Accord.NET

adds support for:

      1) generic image 
	     general idea: The main structure is generic image which is implemented to be similar to EmguCV's)
		 Differences and enhancements:
				Generic image can be used in non-generic way also (useful for many filters and tools, see: some implementations in Accord.Imaging.Extensions project)
				All operations (color conversions) are executed in parallel
			    Easier access to unmanaged data
	  
      2) easy color converting e.g. img.Convert<Hsv, byte>() (parallel)
      
	  3) parallel image processing (many methods are written to utilize multicore CPUs) (SEE performace_results.txt !!!)
      
	  4) optimized convolution (parallel spatial and parallel in frequency domain - FFT)
      
	  5) introduces new extension and compatibility methods (for Bitmap, UnmanagedImage...)
      
	  6) introduces extension pattern method usage (e.g. img.GaussianSmooth(3)...) (parallel)
      
	  7)implements new algorithms (Camshift (parallel), KLT (parallel), ParticleFilter, KalmanFilter)
   
   
roadmap: //TODO list
	  
	implement missing overloads
	write extension methods for existing classes/filters
	overall look and feel
	merge it to Accord.NET
