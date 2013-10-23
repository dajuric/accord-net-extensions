function CompareShapes

im = imread('elipse.bmp');
im = double(im);

% im = [0 0 0 0; 
%       1 1 1 1; 
%       0 0 0 0];

[X, Y] = meshgrid(1:size(im,2), 1:size(im,1));

moments00 = sum(im(:));

centerX = sum(sum((im .* X))) / moments00;
centerY = sum(sum((im .* Y))) / moments00;

centralMoments11 = sum(sum( (X - centerX) .* (Y - centerY) .* im ));
centralMoments20 = sum(sum( (X - centerX).^2 .* im ));
centralMoments02 = sum(sum( (Y - centerY).^2 .* im ));

a = centralMoments20 / moments00;
b = centralMoments11 / moments00;
c = centralMoments02 / moments00;

cov = [a b;...
       b c];

drawEigenVectors(cov, im, centerX, centerY);
end

function drawEigenVectors(cov, im, x, y)

  [eigVec, eigVal] = eig(cov);
  
  lengths = diag(sqrt(eigVal) .* 2);
  orientationRad = atan(eigVec(1,2 ) / eigVec(1,1));
  orientationDeg = orientationRad / 3.14 * 180;
  
  figure;
  imshow(im);
  hold on;
  ellipse(lengths(1), lengths(2), orientationRad, x, y, 'r');
end

