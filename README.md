<table>
  <tr>
    <td> 
      <img src="https://raw.githubusercontent.com/dajuric/accord-net-extensions/master/Deployment/Logo/logo-big.png" alt="" width=150 align="center"> 
    </td>
    
    <td>
    <ul>
     <li>Version: 1.0.0 (release candidate)</li>
     <li>NuGet packages: <a href="https://www.nuget.org/profiles/dajuric"><strong>ready</strong> <i>(pre-release)</i></a></li>
     <li>
       Help: <a href="https://github.com/dajuric/accord-net-extensions/raw/master/Deployment/Documentation/Help/Accord.NET%20Extensions%20Documentation.chm"> Offline </a> - <i>unblock after download!</i></li>
     <li>
        Tutorials: 
        <ul>
          <li><a href="http://www.codeproject.com/Articles/826377/Rapid-Object-Detection-in-Csharp" target="_blank">Fast template matching</a></li>
          
           <li><a href="http://www.codeproject.com/Articles/828012/Introducing-Portable-Video-IO-Library-for-Csharp" target="_blank">Portable Video IO</a></li>
           
            <li><a href="http://www.codeproject.com/Articles/829349/Introducing-Portable-Generic-Image-Library-for-Csh" target="_blank">Portable Generic Image</a></li>
        </ul>
     </li>
    </ul>
    </td>
  </tr>
</table>

<p align="justify">
<b>Accord.NET Extensions</b> is an extension framework for <a href="http://accord-framework.net/"> Accord.NET </a> and <a href ="http://www.aforgenet.com/framework/license.html"> AForge.NET </a>.
The framework is built with extensibility and portability in mind. Most provided algorithms for image processing, computer vision and statistical analysis are made as extensions. New generic image class is not tied to any specific library and 
video IO library, which offers unified interface for camera capture, file and directory reading, is platform abstract.
</p>

The framework is divided in libraries available through NuGet packages. The libraries can be grouped as following:

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
