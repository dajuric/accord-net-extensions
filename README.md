<table>
  <tr>
    <td> 
      <img src="https://raw.githubusercontent.com/dajuric/accord-net-extensions/master/Deployment/Logo/logo-big.png" alt="" width=150 align="center"> 
    </td>
    
    <td>
    <ul>
     <li>Version: 1.2.1 (release candidate)</li>
     <li>NuGet packages: <a href="https://www.nuget.org/profiles/dajuric"><strong>ready</strong> <i>(pre-release)</i></a></li>
     <li>
       Help: <a href="https://github.com/dajuric/accord-net-extensions/raw/master/Deployment/Documentation/Help/Accord.NET%20Extensions%20Documentation.chm"> Offline </a> - <i>unblock after download!</i></li>
     <li>
        Tutorials: 
        <ul>
          <li><a href="http://www.codeproject.com/Articles/826377/Rapid-Object-Detection-in-Csharp" target="_blank">Fast template matching</a></li>
          
         <li><a href="http://www.codeproject.com/Articles/865935/Object-Tracking-Kalman-Filter-with-Ease" target="_blank">Kalman filter</a></li>
          
         <li><a href="http://www.codeproject.com/Articles/865934/Object-Tracking-Particle-filter-with-ease" target="_blank">Particle filter</a></li>
          
           <li><a href="http://www.codeproject.com/Articles/840823/Object-Feature-Tracking-in-Csharp" target="_blank">Object Feature Tracking - KLT Optical Flow</a></li>
          
           <li><a href="http://www.codeproject.com/Articles/828012/Introducing-Portable-Video-IO-Library-for-Csharp" target="_blank">Portable Video IO</a></li>
           
            <li><a href="http://www.codeproject.com/Articles/829349/Introducing-Portable-Generic-Image-Library-for-Csh" target="_blank">Portable Generic Image</a></li>
        </ul>
     </li>
    </ul>
    </td>
  </tr>
  <tr>
    <td><a href="https://gitter.im/dajuric/accord-net-extensions?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge" title="Join the chat at https://gitter.im/dajuric/accord-net-extensions"><img src="https://badges.gitter.im/Join%20Chat.svg" alt=""/></a></td>
    </td>
  </tr>
</table>

<p align="justify">
<b>Accord.NET Extensions</b> is an extension framework for <a href="http://accord-framework.net/"> Accord.NET </a> and <a href ="http://www.aforgenet.com/framework/license.html"> AForge.NET </a>.
The framework is built with extensibility and portability in mind. Most provided algorithms for image processing, computer vision and statistical analysis are made as extensions. New generic image class is not tied to any specific library and 
video IO library, which offers unified interface for camera capture, file and directory reading, is platform abstract.
</p>

The framework is divided in libraries available through NuGet packages. The libraries can be grouped as following:

<h3>Image processing</h3>

<ul>      
   <li> 
      <b>Accord.Extensions.Imaging.Algorithms</b>
       <p>
        Implements image-processing and computer-vision algorithms.
        Provides extensions for image-processing algorithms implemented in Accord.NET framework and for some image-processing algorithms implemented in AForge.NET framework.
       </p>
       <p>
         <i>samples: (see Accord.Extensions.Imaging.GenericImage)</i>
       </p>
   </li>
	
   <li> 
      <b>Accord.Extensions.Imaging.Algorithms.LINE2D</b>
       <p>
        Implements template matching algorithm (~20x faster than conventional sliding window approach).
       </p>
       <p>
         <i>samples: fast template matching demo</i>
       </p>
   </li>
	 
   <li> 
      <b>Accord.Extensions.Vision</b>
       <p>
         Provides computer vision algorithms: Pyramidal KLT tracker, Camshift, Meanshift.
       </p>
       <p>
         <i>samples: Camshift; pyramidal Lucas-Kanade tracker</i>
       </p>
   </li>
</ul>

<h3>Math libraries</h3>

<ul> 
   <li> 
       <b>Accord.Extensions.Math</b>
       <p>
         Provides extensions for the 2D array, graphs, contour, point transformations. 
         Implements parallel FFT transform. Implements group matching.
       </p>
       <p>
         <i>samples: cardinal spline; contour extrema; graph path search; group matching</i>
       </p>
   </li>
    
   <li> 
      <b>Accord.Extensions.Statistics</b>
       <p>
         Provides classes and extensions for the following filters: Kalman, Particle filter, JPDAF - Joint Probability Data 
         Association Filter. Includes 2D motion models.
       </p>
       <p>
         <i>samples: Kalman: simulation and object tracking (Kalman + Camshift) demo; Particle filtering: color object tracking; 
            template model selection
         </i>
       </p>
   </li>
</ul>

<h3>Support libraries</h3>

<ul> 
   <li> 
       <b>Accord.Extensions.Core</b>
       <p>
          The core of the Accord.NET Extensions Framework.
          Contains classes needed for other libraries: 
	      collections, structures, structures for parallel processing and extensions shared across libraries. 
       </p>
       <p>
         <i>
            samples: collections: sparse matrix, circular list, history, pinned array;
                     element caching (lazy memory cache, LRU cache);
                     method caching;
                     parallel array operations;
                     various extensions
         </i>
       </p>
   </li>
     
   <li> 
       <b>Accord.Extensions.Imaging.GenericImage</b>
       <p>
          Implements slim generic image class and basic extensions (arithmetics). Provides multiple color spaces and conversions 
          between them. The class can be used in non-generic way for developers who prefer AForge's UnmanagedImage style.
          To get compatibility for other image types install appropriate extension - NuGet package (e.g. 
          Imaging.BitmapInterop).
       </p>
       <p>
         <i>AForge, OpenCV, Bitmap, array interoperability demos; performance, automatic color conversion, various extensions</i>
       </p>
   </li> 
	 
   <li> 
      <b>Accord.Extensions.Imaging.AForgeInterop</b>
       <p>
        Provides extensions for easy interoperability between generic image and AForge UnmanagedImage.
       </p>
       <p>
         <i>samples: (see Accord.Extensions.Imaging.GenericImage)</i>
       </p>
   </li>
	 
   <li> 
      <b>Accord.Extensions.Imaging.BitmapInterop</b>
       <p>
         Provides extensions for interoperability with System.Drawing.Bitmap, Point, PointF, Color and drawing extensions.
       </p>
       <p>
         <i>samples: (see Accord.Extensions.Imaging.GenericImage)</i>
       </p>
   </li>
	 
   <li> 
      <b>Accord.Extensions.Vision.IO</b>
       <p>
        Provides unified API for IO video access: web-camera support, various video-format reading / writing, image-directory reader.
        All operations are stream-like and are abstracted therefore do not depend on actual video source.
        The library is made in platform-abstract fashion.
       </p>
       <p>
         <i>samples: basic capture; capture and recording; video extraction</i>
       </p>
   </li>
</ul>

<h2>Getting started</h2>
<p align="justify">
   The official way for package installation is the installation over NuGet. Just before you type "Accord.Extensions" or "Accord" (then you need to scroll a bit), please select "Include Pre-release" in the Visual Studio drop-down, due to pre-release version of the framework.
</p>

<h2>Final word</h2>
<p align="justify">
  <ul>
    <li>
     If you like the project please <b>star it</b> in order to help to spread the word. That way you will make the framework more significant and in the same time you will motivate me to improve it, so the benefit is mutual.
    </li>
    
    <li>
       If you have any questions, comments or you would like to propose an enhacement please leave the message on Github, or write to: darko.juric2 [at] gmail.com
    </li>
  </ul>
</p>
