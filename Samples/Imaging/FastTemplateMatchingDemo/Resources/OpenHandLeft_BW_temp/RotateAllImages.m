function RotateAllImages()

   imgFiles = dir('*.bmp');
   
   for imgFileIdx = 1:1:length(imgFiles)
       fprintf('Obraðujem %d. sliku...\n', imgFileIdx);
       
       imgFileName = imgFiles(imgFileIdx).name;
       RotateImage(imgFileName);      
   end

end