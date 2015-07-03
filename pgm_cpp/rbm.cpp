/*
This code implements an RBM.
It is written with interop with the F# library pgm in mind.
It is thus in C style with some C++ features used, such as:

  std vectors, because fuck the struct hack. They even clean up after themselves.
  function polymorphism

Random number generation: Time seeds, mersenne twister are used.
To sample, I create lots of <random> bernoulli distributions.
OpenCL training methods are a high priority.

// new or malloc? Does it make a difference when copying to GPU?
*/

#include "rbm.h"
#include <vector>
#include <random> // for random rbm initialization
#include <functional> // for bind
#include <algorithm>    // std::generate_n
#include <math.h> // exp

#include <iostream>
using namespace std;


namespace pgm_cpp {
	typedef std::vector<double> dv;

	double sigmoid(double x) {
	  return 1. / (1. + exp(-1. * x));
	}

	// These are globals to save some computation. Used for sampling later.
	std::random_device rd;
	std::mt19937 gen(rd());


	int get_number_of_weights(rbm* r) {
	  return (r->nv * r->nh) + r->nv + r->nh;
	}

	int get_number_of_weights(int nv, int nh) {
	  return (nv * nh) + nv + nh;
	}

	// constructors! no full constructor function, just use struct immediately.
	// with initial weights
	void mk_rbm(rbm* res_rbm, int nv, int nh, std::vector<double> weights) {
	  // initialize the visible and hidden weights to 0s
	  res_rbm->visible = std::vector<int>(nv, 0);
	  res_rbm->hidden = std::vector<int>(nh, 0);
	  res_rbm->weights = weights;
	}

	void mk_rbm(rbm* res_rbm, int nv, int nh, double* weights) {

	  // initialize the visible and hidden weights to 0s
	  res_rbm->visible = std::vector<int>(nv, 0);
	  res_rbm->hidden = std::vector<int>(nh, 0);
	  // initialize the double weights
	  res_rbm->weights.assign(weights, weights + get_number_of_weights(nv, nh));
	}

	// return it, even
	rbm mk_rbm(unsigned int nv, unsigned int nh) {

		// get the randomly initialized weights
		size_t elements = get_number_of_weights(nv, nh);
		std::vector<double> weights(elements);
		//std::uniform_real_distribution<double> distribution(-1.0f, 1.0f); //Values between -1 and 1
		std::normal_distribution<double> distribution(0.0, 1.0);
		std::random_device rd; //seed
		std::mt19937 engine(rd());
		auto generator = std::bind(distribution, engine);
		std::generate_n(weights.begin(), elements, generator);

		std::vector<int> visible = std::vector<int>(nv, 0);
		std::vector<int> hidden = std::vector<int>(nh, 0);

		rbm res = { nv, nh, visible, hidden, weights };
		return res;
	}

	// getters, I suppose
	int get_visible(rbm* r, unsigned int i) {
	  return (i < r->nv) ? r->visible[i] : -1;
	}

	int get_hidden(rbm* r, unsigned int j) {
	  return (j < r->nh) ? r->hidden[j] : -1;
	}

	// not for biases
	double get_weight(rbm* r, unsigned int visible_idx, unsigned int hidden_idx) {
	  if (!(visible_idx < r->nv && hidden_idx < r->nh)) {
		  return -100.;
		}
	  int idx = visible_idx * (r->nh + 1) + hidden_idx;

	  return r->weights[idx];
	}

	double get_weight_visible_bias(rbm* r, unsigned int i) {
	  if (!(i < r->nv)) {
		return -1.;
	  }

	  return r->weights[(r->nh + 1) * i]; // the bias is after the weights
	}

	std::vector<double> get_visible_biases(rbm* r) {
	  std::vector<double> res(r->nv);
	  for(unsigned int i=0; i<r->nv; i++) {
		res[i] = get_weight_visible_bias(r, i);
	  }
	  return res;
	}

	double get_weight_hidden_bias(rbm* r, unsigned int j) {
	  if (!(j < r->nh)) {
		return -1.;
	  }
	  return r->weights[(r->nv + 1) * r->nh + j - 1]; // these biases are at the end
	}

	std::vector<double> get_hidden_biases(rbm* r) {
	  std::vector<double> res(r->nh);
	  for(unsigned int j=0; j<r->nh; j++) {
		res[j] = get_weight_hidden_bias(r, j);
	  }
	  return res;
	}

	/* Get the energy of the current configuration: consists of two unary and a pairwise term
	double get_energy(rbm* r) {

	  std::vector<double> a = get_visible_biases(r);
	  std::vector<double> b = get_hidden_biases(r);

	  // the current neuron values
	  std::vector<double> v = r->visible;
	  std::vector<double> h = r-> hidden;


	  double res = std::inner_product(get_visible_biases(r).begin())

	}
	*/

	// Given a neurons probability of firing to one, samples it (returns 1 or 0)
	int sample(double prob) {
	  std::bernoulli_distribution d(prob);

	  return d(gen);
	}

	// Given a hidden neuron vector, return the probability of a visible unit i
	double visible_probability(rbm* r, unsigned int i, std::vector<int> hidden) {
	  if (!(i < r->nv && hidden.size() == r->nh)) {
		return -1.;
	  }

	  double s = get_weight_hidden_bias(r, i);
	  for (unsigned int j=0; j < r->nh; j++) {
		s+= get_weight(r, i, j) * hidden[j];
	  }

	  return sigmoid(s);
	}

	// Sample a visible neuron, given a hidden neuron vector
	double sample_visible(rbm* r, unsigned int i, std::vector<int> hidden) {
	  return sample(visible_probability(r, i, hidden));
	}

	// Given an input vector, return the probability of a hidden unit j
	double hidden_probability(rbm* r, unsigned int j, std::vector<int> visible) {
	  if (!(j < r->nh && visible.size() == r->nv)) {
		return -1.;
	  }

	  double s = get_weight_visible_bias(r, j);
	  for (unsigned int i=0; i < r->nv; i++) {
		s+= get_weight(r, i, j) * visible[i];
	  }

	  return sigmoid(s);
	}

	// Sample a hidden neuron, given a visible neuron vector
	double sample_hidden(rbm* r, unsigned int j, std::vector<int> visible) {
	  return sample(hidden_probability(r, j, visible));
	}

	// ditto for whole vectors:
	std::vector<double> visible_probability(rbm* r, std::vector<int>hidden) {
	  if (!(hidden.size() == r->nh)) {
		return std::vector<double>(r->nh, -1.);
	  }

	  std::vector<double> res;
	  res.reserve(hidden.size());

	  for (unsigned int i=0; i<r->nv; i++) {
		res[i] = visible_probability(r, i, hidden);
	  }
	  return res;
	}

	std::vector<int> sample_visible(rbm* r, std::vector<int>hidden) {
	  std::vector<double> probs = visible_probability(r, hidden);
	  std::vector<int> res(probs.begin(), probs.end());
	  if (probs[0] < -0.1) {return res;} // error

	  for (unsigned int i = 0; i<r->nv; i++) {
		res[i] = sample(probs[i]);
	  }
	  return res;
	}

	std::vector<double> hidden_probability(rbm* r, std::vector<int> visible) {
	  if (!(visible.size() == r->nv)) {
		return std::vector<double>(r->nv, -1.);
	  }

	  std::vector<double> res;
	  res.reserve(visible.size());

	  for (unsigned int j=0; j<r->nh; j++) {
		res[j] = hidden_probability(r, j, visible);
	  }
	  return res;
	}

	std::vector<int> sample_hidden(rbm* r, std::vector<int>visible) {
	  std::vector<double> probs = hidden_probability(r, visible);
	  std::vector<int> res(probs.begin(), probs.end());
	  if (probs[0] < -0.1) {return res;} // error

	  for (unsigned int j = 0; j<r->nh; j++) {
		res[j] = sample(probs[j]);
	  }
	  return res;
	}

	// contrastive divergence: given a rbm and an input d(TODO: or with a set visible state?)
	// return another rbm with the same weights, but a different sampled hidden and visible states (TODO: or just the new d?)

	// this does two sampling steps for a rbm. TODO: Do one sampling step less please.
	rbm get_next(rbm* r) {
	  // first sample the hidden layer from the visible one

	  std::vector<int> hidden = sample_hidden(r, r->visible);

	  // make a new rbm
	  rbm r2 = *r;

	  std::vector<int> new_visible = sample_visible(r, r->hidden);

	  r2.visible = new_visible;

	  // you don't have to sample the hidden one
	  //std::vector<int> new_hidden = sample_hidden(&r2, r2.visible);
	  //r2.hidden = new_hidden;

	  return r2;
	}

	// given two rbms with set visible layers and a weight, gives the 'gradient' (unscaled)
	double weight_update(rbm* r1, rbm* r2, unsigned int i, unsigned int j) {

	  int v_i = get_visible(r1, i);
	  double prob = hidden_probability(r1, j, r1->visible);

	  int v_i_2 = get_visible(r2, i);
	  double prob2 = hidden_probability(r2, j, r2->visible);


	  double dw = v_i * prob - v_i_2 - prob2;
	
	  return dw;
	}

	std::vector<double> weight_update(rbm* r1, rbm* r2) {
		return std::vector < double > () ;
	}

	std::vector<double> train(rbm* r, std::vector<int> data, int iterations);

	/// API FUNCTIONS
	void mk_rbm(unsigned int nv, unsigned int nh, int* visible, int* hidden, double* weights) {

		rbm r = mk_rbm(nv, nh);


		for (unsigned int i = 0; i < nv; i++) visible[i] = r.visible[i];
		for (unsigned int j = 0; j < nh; j++) hidden[j] = r.hidden[j];
		for (unsigned int k = 0; k < get_number_of_weights(nv, nh); k++) weights[k] = r.weights[k];

		print_rbm(&r);

		printf("Summertime. %d\n", visible[0]);

		// train or something?

	}

	void print_rbm(rbm* r) {

	  std::cout << "Rbm with " << r->nv << " visible and "
				<< r->nh << " hidden units." << std::endl;

	  // visible units
	  std::cout << "visible units:" << std::endl;
	  for (unsigned int i=0; i<r->nv; i++) {

		std::cout << get_visible(r, i);
		if (i != 0 && i % 8 == 0) {
		  std::cout << std::endl;
		}
		else {
		  std::cout << " ";
		}
	  }
	  std::cout << std::endl;

	  // hidden units
	  std::cout << "Hidden units:" << std::endl;
	  for (unsigned int j=0; j<r->nh; j++) {

		std::cout << get_hidden(r, j);
		if (j != 0 && j % 8 == 0) {
		  std::cout << std::endl;
		}
		else {
		  std::cout << " ";
		}
	  }
	  std::cout << std::endl;

	  // weights
	  std::cout << "Weights:" << std::endl;
	  for (unsigned int i=0; i<r->nv; i++) {

		for(unsigned int j=0; j<r->nh; j++) {
			std::cout << get_weight(r, i, j);
			if ((i + j) != 0 && (i + j) % 8 == 0) {
			  std::cout << std::endl;
			}
			else {
			  std::cout << " ";
			}
		 }
	  }
	  std::cout << std::endl;

	  //biases
	  std::cout << "Bias weights visible:" << std::endl;
	  for (unsigned int i=0; i<r->nv; i++) {

		std::cout << get_weight_visible_bias(r, i);
		if (i != 0 && i % 8 == 0) {
		  std::cout << std::endl;
		}
		else {
		  std::cout << " ";
		}
	  }
	  std::cout << std::endl;
	  std::cout << "Bias weights hidden:" << std::endl;
	  for (unsigned int j=0; j<r->nh; j++) {

		std::cout << get_weight_hidden_bias(r, j);
		if (j != 0 && j % 8 == 0) {
		  std::cout << std::endl;
		}
		else {
		  std::cout << " ";
		}
	  }
	  std::cout << std::endl;
	}


}