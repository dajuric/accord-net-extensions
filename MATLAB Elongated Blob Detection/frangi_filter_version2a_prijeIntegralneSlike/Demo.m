%I=double(imread ('vessel.png'));
I=double(255 - rgb2gray(imread ('skin2.bmp')));
[J,Sigmas,Direction, Dxxs, Dxys, Dyys] = FrangiFilter2D(I);

%[XMAX,IMAX,XMIN,IMIN] = extrema2(J);
maxVal = max(J(:));
threshold = maxVal * 0.35;
IMAX = find(J > threshold);

figure, imshow(J,[]);
figure, imshow(I,[]);

%IMAX = IMAX(1:50);
%figure, imshow(label2rgb(imquantize(J, multithresh(J, 5))), [])
DrawEllipses(Dxxs, Dxys, Dyys, Sigmas, IMAX);