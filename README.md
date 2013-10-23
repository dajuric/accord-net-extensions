accord-net-extensions
=====================

Extension library for Accord.NET

adds support for:
   generic image 
       easy color converting e.g. img.Convert<Hsv, byte>()
       parallel image processing (many methods are written to utilize multicore CPUs)
       introduces new extension and compatibility methods (for Bitmap, UnmanagedImage...)
       
   introduces extension pattern method usage (e.g. img.GaussianSmooth(3)...)
   
   implements new algorithms (KLT, Particle filtering, Kalman filter)
   
   
roadmap: //TODO list
  implement ParticleFilter (merge it from another project)
  implement KalmanFilter (merge it from another project)
  
  implement missing overloads
  write extension methods for existing classes/filters
  overall look and feel
  merge it to Accord.NET
