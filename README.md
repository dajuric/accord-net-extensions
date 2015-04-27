<table>
    <tr>
        <td>
            <img src="https://raw.githubusercontent.com/dajuric/accord-net-extensions/master/Deployment/Logo/logo-big.png" alt="Accord.NET Extensions logo" width="150" align="center">
        </td>

        <td>
            <ul>
                <li>Version: 2.0</li>
                <li>NuGet packages: <a href="https://www.nuget.org/profiles/dajuric"><strong>ready</strong></a></li>
                <li>
                    Help: <a href="https://github.com/dajuric/accord-net-extensions/raw/master/Deployment/Documentation/Help/Accord.NET%20Extensions%20Documentation.chm"> Offline </a> - <i>unblock after download!</i>
                </li>
            </ul>
        </td>
    </tr>
	<tr>
		<td></td>
	    <td align="right">
			<a href="https://bitdeli.com/free" title="Bitdeli Badge"> 
				<img src="https://d2weczhvl823v0.cloudfront.net/dajuric/accord-net-extensions/trend.png" alt=""/>
			</a>
		<td>
	</tr>
</table>

<p align="justify">
    <b>Accord.NET Extensions</b> is an extension framework for <a href="http://accord-framework.net/"> Accord.NET </a> and <a href="http://www.aforgenet.com/framework/license.html">AForge.NET</a>.
    The framework sets focus on .NET native array as primary imaging object, offers algorithms build as extensions, and provides unified platform-abstract imaging IO API. The libraries can be grouped as following:
</p>

<h3>Image processing</h3>

<ul>
    <li>
        <p>
            <b>
                <a href="https://www.nuget.org/packages/Accord.Extensions.Imaging.Algorithms/">Accord.Extensions.Imaging.Algorithms package</a>
            </b>
            <br />
            Implements image processing algorithms as .NET array extensions including the Accord.NET algorithms.
        </p>
        <table border="0" cellpadding="1" cellspacing="1">
            <tbody>
                <tr>
                    <td align="center" width="100%">
                        <img alt="Algorithms sample" src="./Deployment/README-resources/algorithms-sample.png" width="500" />
                    </td>
                </tr>
            </tbody>
        </table>
    </li>

    <li>
        <p>
            <b>
                <a href="https://www.nuget.org/packages/Accord.Extensions.Imaging.Algorithms.LINE2D/">Accord.Extensions.Imaging.Algorithms.LINE2D package</a>
            </b>
            <br />
            Implements template matching algorithm (~20x faster than conventional sliding window approach).
        </p>
        <p>
            <b>Tutorials:</b> <a href="http://www.codeproject.com/Articles/826377/Rapid-Object-Detection-in-Csharp" target="_blank">Fast template matching</a>
        </p>

        <table border="0" cellpadding="1" cellspacing="1">
            <tbody>
                <tr>
                    <td align="center" width="100%">
                        <a href="https://www.youtube.com/watch?v=B4-IR8ySrSs" target="_blank">
                            <img alt="Fast template matching" src="./Deployment/README-resources/FastTemplateMatching.jpg" width="250" />
                        </a>
                    </td>
                </tr>
            </tbody>
        </table>
    </li>

    <li>
        <p>
            <b>
                <a href="https://www.nuget.org/packages/Accord.Extensions.Imaging.Algorithms.LINE2D/">Accord.Extensions.Vision package</a>
            </b>
            <br />
            Pyramidal KLT tracker, Camshift, Meanshift
        </p>
        <p>
            <b>Tutorials:</b>
            <a href="http://www.codeproject.com/Articles/840823/Object-Feature-Tracking-in-Csharp" target="_blank">Object Feature Tracking</a>
        </p>

        <table border="0" cellpadding="1" cellspacing="1">
            <tbody>
                <tr>
                    <td align="center" width="50%">
                        <a href="https://www.youtube.com/watch?v=6B_PNDCWtz4" target="_blank">
                            <img alt="KLT Optical flow" src="./Deployment/README-resources/KLTOpticalFlow.jpg" width="250">
                        </a>
                    </td>
                    <td align="center" width="50%">
                        <a href="https://www.youtube.com/watch?v=C-ivmaUHaP4" target="_blank">
                            <img alt="Camshift tracker" src="./Deployment/README-resources/CamshiftTracker.jpg" width="250">
                        </a>
                    </td>
                </tr>
            </tbody>
        </table>
    </li>
</ul>

<h3>Math libraries</h3>

<ul>
    <li>
        <p>
            <b>
                <a href="https://www.nuget.org/packages/Accord.Extensions.Math/">Accord.Extensions.Math package</a>
            </b>
            <br />
            Fluent matrix extensions. Geometry and graph structures and extensions.
        </p>
    </li>

    <li>
        <p>
            <b>
                <a href="https://www.nuget.org/packages/Accord.Extensions.Statistics/">Accord.Extensions.Statistics package</a>
            </b>
            <br />
            Object tracking algorithms: Kalman filter, particle filter, Joint Probability Data Association Filter (JPDAF).
            <br />
        </p>
        <p>
            <b>Tutorials:</b> <a href="http://www.codeproject.com/Articles/865935/Object-Tracking-Kalman-Filter-with-Ease" target="_blank">Kalman filter</a>,
            <a href="http://www.codeproject.com/Articles/865934/Object-Tracking-Particle-filter-with-ease" target="_blank">Particle filter</a>
        </p>

        <table border="0" cellpadding="1" cellspacing="1">
            <tbody>
                <tr>
                    <td align="center" width="50%">
                        <a href="https://www.youtube.com/watch?v=LSoJXCQIAvQ" target="_blank">
                            <img alt="Kalman Filter 2D" src="./Deployment/README-resources/KalmanFiler2D.jpg" width="250">
                        </a>
                    </td>
                    <td align="center" width="50%">
                        <a href="https://www.youtube.com/watch?v=p9FXMf_HRw0" target="_blank">
                            <img alt="Kalman Filter Tracking" src="./Deployment/README-resources/KalmanFilterTracking.jpg" width="250">
                        </a>
                    </td>
                </tr>

                <tr>
                    <td align="center" width="50%">
                        <a href="https://www.youtube.com/watch?v=SV6CmEha51k" target="_blank">
                            <img alt="Particle filter color tracking" src="./Deployment/README-resources/ParticleFilterColorTracking.jpg" width="250">
                        </a>
                    </td>
                    <td align="center" width="50%">
                        <a href="https://www.youtube.com/watch?v=5VWtoimnlto" target="_blank">
                            <img alt="Particle filter template matching" src="./Deployment/README-resources/ParticleFilterTemplateMatching.jpg" width="250">
                        </a>
                    </td>
                </tr>
            </tbody>
        </table>

    </li>
</ul>

<h3>Support libraries</h3>

<ul>
    <li>
        <p>
            <b>
                <a href="https://www.nuget.org/packages/Accord.Extensions.Imaging.GenericImage">Accord.Extensions.Imaging.GenericImage package</a>
            </b>
            <br />
            .NET image array extensions. Color and depth conversions. Slim unmanaged structure for fast pixel manipulation.
        </p>
        <p>
            <b>Tutorials:</b>
            <a href="http://www.codeproject.com/Articles/829349/Introducing-Portable-Generic-Image-Library-for-Csh" target="_blank">Portable Generic Image</a> <em>(update pending)</em>
        </p>

        <table border="0" cellpadding="1" cellspacing="1">
            <tbody>
                <tr>
                    <td align="center" width="100%">
                        <img alt="Generic image sample" src="./Deployment/README-resources/genericImage-sample.png" width="500" />
                    </td>
                </tr>
            </tbody>
        </table>
    </li>

    <li>
        <p>
            <b>
                <a href="https://www.nuget.org/packages/Accord.Extensions.Imaging.Drawing">Accord.Extensions.Imaging.Drawing package</a>
            </b>
            <br />
            .NET image drawing array extensions.
        </p>
        <table border="0" cellpadding="1" cellspacing="1">
            <tbody>
                <tr>
                    <td align="center" width="100%">
                        <img alt="Generic image drawing sample" src="./Deployment/README-resources/genericImage-drawing-sample.png" width="500" />
                    </td>
                </tr>
            </tbody>
        </table>
    </li>

    <li>
        <p>
            <b>
                <a href="https://www.nuget.org/packages/Accord.Extensions.Imaging.IO">Accord.Extensions.Imaging.IO package</a>
            </b>
            <br />
            A unified API for IO video access (camera, file, image directory). Portable image loading/saving.
        </p>
        <p>
            <b>Tutorials:</b>
            <a href="http://www.codeproject.com/Articles/828012/Introducing-Portable-Video-IO-Library-for-Csharp" target="_blank">Portable Imaging IO</a>
            <em>(update pending)</em>
        </p>

        <table border="0" cellpadding="1" cellspacing="1">
            <tbody>
                <tr>
                    <td align="center" width="100%">
                        <img alt="Imaging IO sample" src="./Deployment/README-resources/imagingIO-sample.png" width="500" />
                    </td>
                </tr>
            </tbody>
        </table>
    </li>

    <li>
        <p>
            <b>
                <a href="https://www.nuget.org/packages/Accord.Extensions.Imaging.AForgeInterop">Accord.Extensions.Imaging.AForgeInterop package</a>
            </b>
            <br />
            Interoperability extensions between .NET array and AForge's UnmanagedImage.
        </p>
    </li>

    <li>
        <p>
            <b>
                <a href="https://www.nuget.org/packages/Accord.Extensions.Imaging.BitmapInterop">Accord.Extensions.Imaging.Bitmap package</a>
            </b>
            <br />
            Interoperability extensions between .NET array and Bitmap.
        </p>
    </li>

    <li>
        <p>
            <b>
                <a href="https://www.nuget.org/packages/Accord.Extensions.Core">Accord.Extensions.Core package</a>
            </b>
            <br />
            Element and method caching, parallel extensions, shared structures.
        </p>
    </li>

</ul>

<h2>Getting started</h2>
<p align="justify">
    Browse through some included samples, install NuGet packages and enjoy!
</p>

<h2>Final word</h2>
<p align="justify">
    <ul>
        <li>
            If you like the project please <b>star it</b> in order to help to spread the word.
            That way you will make the framework more significant and in the same time you will motivate me to improve it, so the benefit is mutual.
        </li>

        <li>
            If you have any questions, comments or you would like to propose an enhancement please leave the message on Github, or write to: darko.juric2 [at] gmail.com
        </li>
    </ul>
</p>
