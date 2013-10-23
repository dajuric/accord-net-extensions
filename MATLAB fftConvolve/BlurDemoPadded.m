%Blur Demo

%Import image
origimage = imread('peppers','png');

%Reduce image to 2-D
origimage = origimage(:,:,1);

%Plot image
figure, imagesc(origimage)
axis square
colormap gray
title('Original Image')
set(gca, 'XTick', [], 'YTick', [])

%Blur Kernel
ksize = 31;
kernel = zeros(ksize);

%Gaussian Blur
s = 3;
m = ksize/2;
[X, Y] = meshgrid(1:ksize);
kernel = (1/(2*pi*s^2))*exp(-((X-m).^2 + (Y-m).^2)/(2*s^2));

%Display Kernel
figure, imagesc(kernel)
axis square
title('Blur Kernel')
colormap gray

%Pad image
origimagepad = padimage(origimage, ksize);

%Embed kernel in image that is size of original image + padding
[h, w] = size(origimage);
[h1, w1] = size(origimagepad);
kernelimagepad = zeros(h1,w1);

kernelimagepad(1:ksize, 1:ksize) = kernel;

%Perform 2D FFTs
fftimagepad = fft2(origimagepad);
fftkernelpad = fft2(kernelimagepad);

fftkernelpad(find(fftkernelpad == 0)) = 1e-6;

%Multiply FFTs
fftblurimagepad = fftimagepad.*fftkernelpad;

%Perform Reverse 2D FFT
blurimagepad = ifft2(fftblurimagepad);

%Remove Padding
blurimageunpad = blurimagepad(ksize+1:ksize+h,ksize+1:ksize+w);

%Display Blurred Image
figure, imagesc(blurimageunpad)
%figure, imagesc(blurimagepad)
axis square
title('Blurred Image - with Padding')
colormap gray
set(gca, 'XTick', [], 'YTick', [])


