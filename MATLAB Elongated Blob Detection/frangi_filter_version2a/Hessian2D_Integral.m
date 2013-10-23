function [Dxx,Dxy,Dyy] = Hessian2D(I, fSz)

    Dxx = zeros(size(I));
    Dyy = zeros(size(I));
    Dxy = zeros(size(I));
    
    I = padarray(I, [round(fSz/2), round(fSz/2)], 'replicate');

    step = 1;
    integralIm = integralImage(I);
    
    for row = 1: step: (size(I, 1) - fSz-1) 
       for col = 1: step: (size(I, 2) - fSz-1)

           Dxx(row, col) = GetDxx(integralIm, row, col, fSz);
           Dyy(row, col) = GetDyy(integralIm, row, col, fSz);
           %Dxy(row, col) = GetDxy(integralIm, row, col, fSz);

       end 
    end

end

function sum = GetDxy(integralIm, row, col, size)

   b = floor(size / 2);
   sumUpperLeft   =  GetRectangleSum(integralIm, row,     col,     b, b);
   sumBottomLeft  =  GetRectangleSum(integralIm, row + b, col,     b, b);
   sumUpperRight  =  GetRectangleSum(integralIm, row,     col + b, b, b);
   sumBottomRight =  GetRectangleSum(integralIm, row + b, col + b, b, b);
   
   sum = sumUpperLeft + sumBottomRight - sumUpperRight - sumBottomLeft;
end

function sum = GetDyy(integralIm, row, col, size)

   b = floor(size / 3);
   sumUpper =  GetRectangleSum(integralIm, row,        col, size, b);
   sumCenter = GetRectangleSum(integralIm, row + b,    col, size, b);
   sumBottom = GetRectangleSum(integralIm, row + 2*b,  col, size, b);
   
   sum = 2 * sumCenter - sumUpper - sumBottom;
   
   if sum > 0
     sum = sum / (2 * sumCenter);
   else
     sum = 0;
   end
end

function sum = GetDxx(integralIm, row, col, size)

   b = floor(size / 3);
   sumLeft =   GetRectangleSum(integralIm, row, col,       b, size);
   sumCenter = GetRectangleSum(integralIm, row, col + b,   b, size);
   sumRight =  GetRectangleSum(integralIm, row, col + 2*b, b, size);
   
   sum = 2 * sumCenter - sumLeft - sumRight;
   
   if sum > 0
     sum = sum / (2 * sumCenter);
   else
     sum = 0;
   end
   
end

function sum = GetRectangleSum(integralIm, row, col, width, height)
    
    x1 = col;
    y1 = row;
    x2 = col + width - 1;
    y2 = row + height - 1;
            
    sum = integralIm(y2, x2) + integralIm(y1, x1) - integralIm(y1, x2) - integralIm(y2, x1);
end