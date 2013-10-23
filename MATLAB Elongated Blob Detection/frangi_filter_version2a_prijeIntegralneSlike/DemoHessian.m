function DemoHessian()

I = im2double(255 - rgb2gray(imread('skin.bmp')));
%I = imresize(I, [480 640]);

figure, imshow(I, []);

sigma = 5;
[Dxx,Dxy,Dyy] = Hessian2D(I,sigma);

% Correct for scale
Dxx = (sigma^2)*Dxx;
Dxy = (sigma^2)*Dxy;
Dyy = (sigma^2)*Dyy;

blob = sigma * (Dxx + Dyy);
ridge = blob;
%ridge = sigma^(3/2) * ((Dxx-Dyy).^2 + 4*Dxy.^2);

%imshow(Dyy, []);
%return;

%[XMAX,IMAX,XMIN,IMIN] = extrema2(ridge);
threshold = 0.9 * max(max(ridge));
%imshow(ridge > threshold, []);
IMAX = find(ridge > threshold);

%figure, imshow(blob,[]);
figure, imshow(ridge,[]);
%figure, imshow(I);


%IMAX=IMAX(1:10);
extrema = zeros(size(ridge));
extrema(IMAX) = 1;
%figure, imshow(extrema, []);

hold on; 
sigmas = ones(size(I));
sigmas = sigmas * sigma * 3;
DrawEllipses(Dxx, Dxy, Dyy, sigmas, IMAX);
end
