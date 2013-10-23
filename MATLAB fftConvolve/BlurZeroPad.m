A = double(imread('sampleFFT.jpg')); % image
B = fspecial('gaussian', [15 15], 7); % some 2D filter function

[m,n] = size(A);
[mb,nb] = size(B); 
% output size 
mm = m + mb - 1;
nn = n + nb - 1;

% padding constants (for output of size == size(A))
padC_m = ceil((mb-1)./2);
padC_n = ceil((nb-1)./2);

A_padded = zeros(mm, nn);
A_padded(padC_m+1:padC_m+m, padC_n+1:padC_n+n) = A;

A_padded = double(imread('A_padded.bmp'));

B_resized = zeros(size(A_padded));
B_resized(1:mb, 1:nb) = B;

% pad, multiply and transform back
%C = ifft2(fft2(A,mm,nn).* fft2(B,mm,nn));
fftA_padded = fft2(A_padded);
fftB_resized = fft2(B_resized);
fftConvolved = fftA_padded.* fftB_resized;
C = ifft2(fftConvolved);


% frequency-domain convolution result
D = C(mb:m+mb-1, nb:n+nb-1); 
figure; imshow(uint8(D));