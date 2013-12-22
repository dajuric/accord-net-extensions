#include <set>

extern "C" __declspec(dllexport) void FindIndexesGreaterThan(short* srcAddr, int numOfElems, short threshold, int* indexes);

void FindIndexesGreaterThan(short* srcAddr, int numOfElems, short threshold, int* indexes)
{
	__m128i thresholdSSE = _mm_set1_epi16(threshold);

	int i = 0;

	while(i <= numOfElems)
	{
	   __m128i srcSSE = _mm_lddqu_si128(reinterpret_cast<__m128i*>(srcAddr + i));

	   __m128i compResult = _mm_cmpgt_epi16(srcSSE, thresholdSSE); //compare
	    unsigned short byteFlags = _mm_movemask_epi8(compResult);  //compress to flag per bit for each byte

	   

	   i += 16;
	}
}

int main5()
{
	short arr[8];
	arr[0]=10; arr[1]=5; arr[2]=4; arr[3]=-1; arr[4]=0; arr[5]=0; arr[6]=0; arr[7]=0; 

	short threshold = 5;

	FindIndexesGreaterThan(arr, 1, threshold, 0);

	return 0;
}