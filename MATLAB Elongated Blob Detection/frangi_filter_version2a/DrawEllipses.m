function DrawElipses(Dxx, Dxy, Dyy, sigmas, indicies) 
   
   i = 1;
   for idx = indicies'    
      cov = [Dxx(idx) Dxy(idx); Dxy(idx) Dyy(idx)];
      [row, col] = ind2sub(size(Dxx), idx);
      drawEigenVectors(cov, col, row, sigmas(i));   
      
      i=i+1;
   end   
end

function drawEigenVectors(cov, x, y, sigma)

  [eigVec, eigVal] = eig(cov);
  
  lengths = diag(sqrt(abs(eigVal)) .* 2) *  sigma * 10;
  orientationRad = atan2(eigVec(1,1 ), eigVec(1,2)) + pi/2;
  %orientationDeg = orientationRad / 3.14 * 180;
  
  ellipse(lengths(1), lengths(2), orientationRad, x, y, 'g');
  %ellipse(5, 5, 0, x, y, 'r');
end