﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.DataExt
{
    class cData2
    {

        public byte[] data4aa = {

     

244, 68, 10, 0,    24, 6, //Tent maybe?
   1,8,47,2,
   2,242,46,2,
   /*2,88,195,6,
   3,76,51,1,
   4,16,59,2,
   5,144,62,2,
   6,54,39,1,
   7,46,82,1,
   8,58,82,1,
   9,10,51,1,
   10,196,54,2,
   12,164,70,2,
   13,248,50,1,
   14,44,47,1,
   15,198,54,1,
   16,170,196,12,
244, 68, 8, 0, 

24, 7,  
  10,64,11,2,13,1,/* 
  4, 143, 
  5,82, 
  6,  1, 
  7,68,  
  8, 22,  
  9,32, 
  10,120,
  11,74,
  12,8,
  13,9,
  15,144,
  16,20,
  17,220,
  23,132,
  27,24,
  29,132,
  36,4,
  37,2,
  39,28,
  40,9,
  41,130,
  44,128,
  45,1,
  46,233,
  47,137,
  48,4,
  52,200,
  53,170,
  54,3,
  55,31,
  56,32,
  59,128,
  60,32,
  62,34,
  66,1,
  68,144,
  70,12,
  74,64,
  75,1,
  76,4,
  85,2,
  90,1,
  107,8 */                             };


        /*

// tent items
244, 68, 163, 1, 62, 4, 
//***************************************
138, 152, 26, 0, 
1, 0, 161, 148, 43, 0,     
0, 0, 39, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 3,                    
0, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                     
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                     
0, 0, 0, 0, 0, 2, 0, 167, 148, 30, 0, 0, 0, 39, 0, 0,               
0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                     
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                     
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                     
3, 0, 170, 148, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                 
0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                     
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                     
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 0, 118, 152, 40,                
0, 0, 0, 38, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0,                    
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                     
0, 0, 0, 0, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                    
0, 0, 0, 0, 0, 0, 5, 0, 139, 148, 39, 0, 0, 0, 39, 0,               
0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                     
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10,                    
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                     
0, 6, 0, 126, 148, 37, 0, 0, 0, 38, 0, 0, 0, 1, 0, 0,               
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                     
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10, 0, 0, 0, 0, 0,                    
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 0, 148, 148,                 
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0,                     
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                     
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                     
0, 0, 0, 0, 0, 0, 0, 
*/

        public byte[] data4b = {
244, 68, 3, 0, 5, 14, 2, 
244, 68, 3, 0, 5, 16, 2, 
244, 68, 11, 0, 23, 140, 3,63,248,60,7,105,51,228,64, //time
244, 68, 11, 0, 25, 44, 1,63,248,60,7,105,51,228,64,  //time
244,68,7,0,23,106,1,1,0,0,0,
244, 68, 3, 0, 23, 160, 3, 
244, 68, 3, 0, 75, 7,1,
244,68,24,0,66,1,1,27,43,0,0,0,0,0,0,0,0,2,25,43,0,0,0,0,0,0,0,0,
244, 68, 5, 0, 5, 13, 1, 0, 0, 
244, 68, 5, 0, 5, 13, 2, 0, 0, 
244, 68, 5, 0, 5, 13, 3, 0, 0, 
244, 68, 5, 0, 5, 13, 4, 0, 0, 
244, 68, 5, 0, 5, 13, 5, 0, 0, 
244, 68, 5, 0, 5, 13, 6, 0, 0, 
244, 68, 5, 0, 5, 13, 7, 0, 0,                
244, 68, 5, 0, 5, 13, 8, 0, 0, 
244, 68, 5, 0, 5, 13, 9, 0, 0, 
244, 68, 5, 0, 5, 13, 10,0, 0, 
244, 68, 5, 0, 5, 24, 1, 0, 0, 
244, 68, 5, 0, 5, 24, 2, 0, 0, 
244, 68, 5, 0, 5, 24, 3, 0, 0, 
244, 68, 5, 0, 5, 24, 4, 0, 0, 
244, 68, 5, 0, 5, 24, 5, 0, 0, 
244, 68, 5, 0, 5, 24, 6, 0, 0,
244, 68, 5, 0, 5, 24, 7, 0, 0, 
244, 68, 5, 0, 5, 24, 8, 0, 0, 
244, 68, 5, 0, 5, 24, 9, 0, 0, 
244, 68, 5, 0, 5, 24, 10, 0, 0, 
244, 68, 5, 0, 23, 162, 2, 0, 0, 
244, 68, 6, 0, 26, 10, 0, 0, 0, 0, 
244, 68, 4, 0, 23, 204, 1, 0,  
244, 68, 8, 0, 23, 208, 2, 3, 0, 0, 0, 0, 
244, 68, 8, 0, 23, 208, 2, 4, 0, 0, 0, 0, 
244, 68, 2, 0, 1, 11, 
244, 68, 3, 0, 15, 19,2,3,4 };
    }
}