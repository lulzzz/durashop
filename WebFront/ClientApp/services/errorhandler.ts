export default class ErrorHandler{
    static Handle(dispatch: any, objToDispatch: any, error: any){
        alert('FAILED: ' + error);
        dispatch(objToDispatch);
    }
}