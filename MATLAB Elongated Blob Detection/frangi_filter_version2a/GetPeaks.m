function peaksIm = GetPeaks(responseIm, layerIm)

  quantizedIm = imquantize(responseIm, multithresh(responseIm, 15));
  
  peaksIm = label2rgb(quantizedIm);
  return;
  
  quantizedIm(quantizedIm < 9) = 0;
  
  peaksImR = layerIm;
  peaksImR(quantizedIm > 0) = 255;  
  
  peaksImG = layerIm;
  peaksImG(quantizedIm > 0) = 0; 
  
  peaksImB = layerIm;
  peaksImB(quantizedIm > 0) = 0; 
  
  peaksIm(:,:,1) = peaksImR;
  peaksIm(:,:,2) = peaksImG;
  peaksIm(:,:,3) = peaksImB;
end