function Ipad = padimage(I, p)
%This function pads the edges of an image to minimize edge effects 
%during convolutions and Fourier transforms. %Inputs %I - image to pad %p - size of padding around image %Output %Ipad - padded image 

%Find size of image
[h, w] = size(I); 

%Pad edges
Ipad = zeros(h+2*p, w+2*p);  

%Middle
Ipad(p+1:p+h, p+1:p+w) = I;
%return;

%Top and Bottom
Ipad(1:p, p+1:p+w) = repmat(I(1,1:end), p, 1);
Ipad(p+h+1:end, p+1:p+w) = repmat(I(end,1:end), p, 1); 

%Left and Right
Ipad(p+1:p+h, 1:p) = repmat(I(1:end,1), 1, p);
Ipad(p+1:p+h, p+w+1:end) = repmat(I(1:end,end), 1, p); 

%Corners
Ipad(1:p, 1:p) = I(1,1); %Top-left
Ipad(1:p, p+w+1:end) = I(1,end); %Top-right
Ipad(p+h+1:end, 1:p) = I(end,1); %Bottom-left
Ipad(p+h+1:end,p+w+1:end) = I(end,end); %Bottom-right