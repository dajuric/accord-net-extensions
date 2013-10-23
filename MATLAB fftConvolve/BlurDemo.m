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

%Embed kernel in image that is size of original image
[h, w] = size(origimage);
kernelimage = zeros(h,w);
kernelimage(1:ksize, 1:ksize) = kernel;

%Perform 2D FFTs
fftimage = fft2(double(origimage));
fftkernel = fft2(kernelimage);

%Set all zero values to minimum value
fftkernel(find(fftkernel == 0)) = 1e-6;

%Multiply FFTs
fftblurimage = fftimage.*fftkernel;

%Perform Inverse 2D FFT
blurimage = ifft2(fftblurimage);

%Display Blurred Image
figure, imagesc(blurimage)
axis square
title('Blurred Image')
colormap gray
set(gca, 'XTick', [], 'YTick', [])


