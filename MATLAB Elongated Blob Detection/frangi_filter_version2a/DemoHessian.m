function DemoHessian()

I = im2double(rgb2gray(imread('proba.bmp')));
%I = imresize(I, [120 240]);

%subplot(3,2,1);
%title('Original');
%imshow(I, []);

sigma = 7;
[Dxx,Dxy,Dyy] = Hessian2D(I,sigma);

% Correct for scale
%Dxx = (sigma^2)*Dxx;
%Dxy = (sigma^2)*Dxy;
%Dyy = (sigma^2)*Dyy;

%Dxx = Standardize(Dxx);
%Dxy = Standardize(Dxy);
%Dyy = Standardize(Dyy);

blob = sigma * (Dxx + Dyy);
ridge = sigma^(3/2) * ((Dxx-Dyy).^2 + 4*Dxy.^2);

peaks = GetPeaks(blob, I);
imshow(peaks, []);
%return;
%DrawEllipses(Dxx, Dxy, Dyy, [sigma], sub2ind(size(I), 510, 350));
%return;

% subplot(3,2,2);
% title('Dxx');
% imshow(Dxx, []);
% 
% subplot(3,2,3);
% title('Dxy');
% imshow(Dxy, []);
% 
% subplot(3,2,4);
% title('Dyy');
% imshow(Dyy, []);
% 
% subplot(3,2,5);
% title('Blob');
% imshow(blob, []);
% 
% subplot(3,2,6);
% title('Ridge');
% imshow(ridge, []);
% 
% return;

%[XMAX,IMAX,XMIN,IMIN] = extrema2(ridge);

%figure, imshow(blob,[]);
%figure, imshow(ridge,[]);
%figure, imshow(I);


%IMAX=IMAX(1:10);
%extrema = zeros(size(ridge));
%extrema(IMAX) = 1;
%figure, imshow(extrema, []);

th = max(max(blob)) * 0.9;
IMAX = find(blob > th);

%hold on; 
sigmas = ones(size(I));
sigmas = sigmas * sigma * 3;
DrawEllipses(Dxx, Dxy, Dyy, sigmas, IMAX);
end


function n = Standardize(I)
  
  n = (I - mean(I(:))) / std(I(:));

end