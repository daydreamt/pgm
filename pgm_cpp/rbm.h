//pgm-cpp.dll
//TODO: export the API for testing?

#ifdef PGM_CPP_EXPORTS
#define PGM_CPP_API __declspec(dllexport) 
#else
#define PGM_CPP_API __declspec(dllimport) 
#endif

#include <stdexcept>
#include <vector>


using namespace std;

namespace pgm_cpp {

	extern "C" { __declspec(dllexport) double sigmoid(double x); }

	typedef struct rbm {
		unsigned int nv;  // the number of visible units
		unsigned int nh;  // the number of hidden units;

		std::vector<int> visible;
		// visible unit state vector

		std::vector<int> hidden;
		// hidden unit state vector

		// weight vector
		// they are addressed by the visible units (since they are symmetric)
		// and the addressing happens like that: first we have nh + 1(the bias)
		// weights for the first hidden unit weights, then nh + 1 for the second, etc
		// and at the end we have nh for the hidden unit bias weights
		std::vector<double> weights;
	}rbm;


	// this actually returns the visible, hidden and weights ;-)
	extern "C" __declspec(dllexport) void mk_rbm(unsigned int nv, unsigned int nh, int* visible, int* hidden, double* weights);
	extern "C" __declspec(dllexport) void train(unsigned int nv, unsigned int nh, double* weights, int* data, int datalen, int alpha, int iterations);

	// getters, do not export to api, but testing?
	int get_visible(rbm* r, unsigned int i);
	double get_weight_visible_bias(rbm* r, unsigned int i);
	int get_hidden(rbm* r, unsigned int j);

	// not for biases
	double get_weight(rbm* r, unsigned int visible_idx, unsigned int hidden_idx); 

	//sampling
	double sample_visible(rbm* r, unsigned int i, std::vector<int> hidden); 
	double sample_hidden(rbm* r, unsigned int j, std::vector<int> visible); 

	double visible_probability(rbm* r, unsigned int i, std::vector<int> hidden); 
	double hidden_probability(rbm* r, unsigned int j, std::vector<int> visible); 

	// not exportable anyway...
	std::vector<double> visible_probability(rbm* r, std::vector<int>hidden);
	std::vector<int> sample_visible(rbm* r, std::vector<int>hidden);

	std::vector<double> hidden_probability(rbm* r, std::vector<int> visible);
	std::vector<int> sample_hidden(rbm* r, std::vector<int>visible);

	// training
	rbm get_next(rbm* r);
	double weight_update(rbm* r1, rbm* r2, unsigned int i, unsigned int j);
	std::vector<double> weight_update(rbm* r1, rbm* r2);

	// misc
	void print_rbm(rbm* r);
}
