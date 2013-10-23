%I=double(imread ('vessel.png'));
I=double(rgb2gray(imread ('hand2.jpeg')));
%I = imresize(I, [120 240]);
[J,Sigmas,Direction, Dxxs, Dxys, Dyys] = FrangiFilter2D(I, [25 25]);

%[XMAX,IMAX,XMIN,IMIN] = extrema2(J);
%maxVal = max(J(:));
%threshold = maxVal * 0.35;
%IMAX = find(J > threshold);

%figure, imshow(J,[]);
%figure, imshow(I,[]);

peaks = GetPeaks(J, I);
imshow(peaks, []);

%IMAX = IMAX(1:50);
%figure, imshow(label2rgb(imquantize(J, multithresh(J, 5))), [])
%DrawEllipses(Dxxs, Dxys, Dyys, Sigmas, IMAX);