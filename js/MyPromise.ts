export class MyPromise {
  private _onSuccess: [];
  private _resolvedValue;

  private _onRejected: [];
  private _rejectedValue;

  constructor(initialiser: (resolve, reject) => void) {
    this._onSuccess = [];
    this._onRejected = [];
    initialiser(
      value => this._resolve(value),
      error => this._reject(error)
    );
  }

  private _resolve(value) {
    if (!this._isComplete) {
    }
  }

  private _reject(value) {
    return this._resolvedValue != undefined || this._rejectedValue != undefined;
  }

  private get _isComplete() {
    return this._resolvedValue != undefined || this._rejectedValue != undefined;
  }

  then(onSuccess) {}
  catch(onFail) {}
}
