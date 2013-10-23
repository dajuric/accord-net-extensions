function [Dxx,Dxy,Dyy] = Hessian2D(I,Sigma)
%  This function Hessian2 Filters the image with 2nd derivatives of a 
%  Gaussian with parameter Sigma.
% 
% [Dxx,Dxy,Dyy] = Hessian2(I,Sigma);
% 
% inputs,
%   I : The image, class preferable double or single
%   Sigma : The sigma of the gaussian kernel used
%
% outputs,
%   Dxx, Dxy, Dyy: The 2nd derivatives
%
% example,
%   I = im2double(imread('moon.tif'));
%   [Dxx,Dxy,Dyy] = Hessian2(I,2);
%   figure, imshow(Dxx,[]);
%
% Function is written by D.Kroon University of Twente (June 2009)

if nargin < 2, Sigma = 1; end

% Make kernel coordinates
[X,Y]   = ndgrid(-round(3*Sigma):round(3*Sigma));

% Build the gaussian 2nd derivatives filters
DGaussxx = 1/(2*pi*Sigma^4) * (X.^2/Sigma^2 - 1) .* exp(-(X.^2 + Y.^2)/(2*Sigma^2));
DGaussxy = 1/(2*pi*Sigma^6) * (X .* Y)           .* exp(-(X.^2 + Y.^2)/(2*Sigma^2));
DGaussyy = DGaussxx';

Dxx = imfilter(I,DGaussxx,'conv', 'symmetric');
Dxy = imfilter(I,DGaussxy,'conv', 'symmetric');
Dyy = imfilter(I,DGaussyy,'conv', 'symmetric');
%Dxx = conv2(I, DGaussxx, 'same');%
%Dxy = conv2(I, DGaussxy, 'same');
%Dyy = conv2(I, DGaussyy, 'same');
