import json

class Value(object):
    def __init__(self, value):
        self.value = value

    def __iter__(self):
        yield from {
            "value": self.Value
        }.items

    def __str__(self):
        return json.dumps(dict(self), ensure_ascii=False)

    def toJson(self):
        return self.__str__()


class Parameters(object):
    def __init__(self, value: Value):
        self.Value = value
        
    def __str__(self):
        return json.dumps(dict(self), ensure_ascii=False)

    def toJson(self):
        return self.__str__()