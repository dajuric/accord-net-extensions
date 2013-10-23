function [outIm,whatScale,Direction, Dxxs, Dxys, Dyys] = FrangiFilter2D(I, sigmas)

sigmas = sort(sigmas, 'ascend');

% Make matrices to store all filterd images
ALLfiltered=zeros([size(I) length(sigmas)]);
ALLangles=zeros([size(I) length(sigmas)]);

Dxxs = zeros([size(I) length(sigmas)]);
Dxys =zeros([size(I) length(sigmas)]);
Dyys =zeros([size(I) length(sigmas)]);

% Frangi filter for all sigmas
for i = 1:length(sigmas),    
    % Make 2D hessian
    [Dxx,Dxy,Dyy] = Hessian2D(I,sigmas(i));
    %Dxx = imquantize(Dxx, multithresh(Dxx, 8));
    %Dxy = imquantize(Dxy, multithresh(Dxy, 8));
    %Dyy = imquantize(Dyy, multithresh(Dyy, 8));
    
    % Correct for scale
    %Dxx = (sigmas(i)^2)*Dxx;
    %Dxy = (sigmas(i)^2)*Dxy;
    %Dyy = (sigmas(i)^2)*Dyy;
   
    % Calculate (abs sorted) eigenvalues and vectors
    [Lambda2,Lambda1,Ix,Iy]=eig2image(Dxx,Dxy,Dyy);

    % Compute the direction of the minor eigenvector
    angles = atan2(Ix,Iy);

    % Compute some similarity measures
    Lambda1(Lambda1==0) = eps;
    
    % Compute the output image
    Ifiltered = Dxx+Dyy;
    %Ifiltered = (Dxx-Dyy).^2+ 4*Dxy.^2;
    
    Ifiltered(Lambda1>0)=0;
    
    % store the results in 3D matrices
    ALLfiltered(:,:,i) = Ifiltered;
    ALLangles(:,:,i) = angles;
    
    Dxxs(:,:,i)=Dxx;
    Dxys(:,:,i)=Dxy;
    Dyys(:,:,i)=Dyy;
end

% Return for every pixel the value of the scale(sigma) with the maximum 
% output pixel value
if length(sigmas) > 1,
    [outIm,whatScale] = max(ALLfiltered,[],3);
    outIm = reshape(outIm,size(I));
    if(nargout>1)
        whatScale = reshape(whatScale,size(I));
        
        for sigma=sigmas
            whatScale(whatScale==sigma)=sigma;
        end       
    end
    if(nargout>2)
        Direction = reshape(ALLangles((1:numel(I))'+(whatScale(:)-1)*numel(I)),size(I));
    end
else
    outIm = reshape(ALLfiltered,size(I));
    if(nargout>1)
            whatScale = ones(size(I));
    end
    if(nargout>2)
        Direction = reshape(ALLangles,size(I));
    end
end
